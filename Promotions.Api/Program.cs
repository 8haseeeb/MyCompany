using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog; 
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using Promotions.Api.Controllers;
using Promotions.Api.Middleware;
using Promotions.Api.Security;
using Promotions.Application;
using Promotions.Application.Common;
using Promotions.Application.Participant.Interfaces;
using Promotions.Infrastructure;
using Promotions.Infrastructure.Persistence.External;
using Promotions.Infrastructure.Persistence.Repositories;
using Serilog;
using Promotions.Infrastructure.Persistence;
using FluentValidation;
using Promotions.Application.Common.Behaviors;
using Promotions.Application.Common.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "Promotions.Api");
builder.Services.AddLoggingLevelSwitch(); // Enable dynamic logging level control
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
    builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });
else
    builder.Services.AddDistributedMemoryCache(); // Single-node fallback when Redis not configured
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
var promoAiConn = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(promoAiConn) && !promoAiConn.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddApplicationInsightsTelemetry(o => o.ConnectionString = promoAiConn);

// OpenTelemetry: receive W3C traceparent from Gateway; export traces to Application Insights.
var otelConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrEmpty(otelConnectionString) && !otelConnectionString.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = otelConnectionString);
        });
}
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "your valid token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Health Monitoring Service
builder.Services.AddHostedService<Promotions.Api.Services.HealthMonitoringService>();


builder.Services.AddAutoMapper(typeof(AssemblyMarker).Assembly);

builder.Services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(AssemblyMarker).Assembly, typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var ssoConnectionString = builder.Configuration.GetConnectionString("SsoConnection");

builder.Services.AddInfrastructure(connectionString!);
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString!, name: "PromotionsDb", tags: new[] { "ready", "db" });

// Add SsoDbContext for Session Validation
if (!string.IsNullOrEmpty(ssoConnectionString))
{
    builder.Services.AddDbContext<SsoDbContext>(options =>
        options.UseSqlServer(ssoConnectionString));
}
else
{
    // If SsoConnection not configured, register a no-op to avoid DI crash
    // The middleware will block all authenticated requests with DB_ERROR
    builder.Services.AddDbContext<SsoDbContext>(options =>
        options.UseSqlServer("Server=localhost;Database=MISSING_SSO_CONNECTION_PLACEHOLDER;Trusted_Connection=True;"));
    Log.Warning("[STARTUP] SsoConnection is NULL - session validation will block all requests!");
}


var app = builder.Build();

// Log connection info to Serilog
Log.Information("[STARTUP] SsoConnection env var: {SsoEnv}",
    Environment.GetEnvironmentVariable("ConnectionStrings__SsoConnection") ?? "NOT SET - using appsettings");

// --- DATABASE AUTO-MIGRATION WITH RETRY ---
// New scope + DbContext per attempt: reusing one context after SqlException can leave connections invalid,
// causing EF to retry CREATE DATABASE (1801) forever even when PromotionsDb already exists.
{
    var migrationLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Promotions.Migration");
    const int maxAttempts = 15;
    var succeeded = false;

    for (var attempt = 1; attempt <= maxAttempts && !succeeded; attempt++)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PromotionsDbContext>();

        try
        {
            if (!context.Database.IsSqlServer())
                break;

            migrationLogger.LogInformation("Applying Promotions database migrations (attempt {Attempt}/{Max})...", attempt, maxAttempts);
            await context.Database.MigrateAsync();
            migrationLogger.LogInformation("Promotions database migrated successfully.");
            succeeded = true;
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801)
        {
            // 1801 = Database already exists.
            // If EF thinks it needs to create it, but it's already there, we check if we can actually use it.
            migrationLogger.LogWarning("[Migration] SqlException 1801 (database already exists). Attempt {Attempt}/{Max}.", attempt, maxAttempts);
            
            var canConnect = await context.Database.CanConnectAsync();
            if (canConnect)
            {
                migrationLogger.LogInformation("[Migration] PromotionsDb already exists and is reachable. Proceeding to apply migrations...");
                try 
                {
                    await context.Database.MigrateAsync();
                    succeeded = true;
                    migrationLogger.LogInformation("Promotions database migrated successfully.");
                }
                catch (Exception migrateEx)
                {
                    migrationLogger.LogError(migrateEx, "[Migration] Connection is OK but MigrateAsync failed. Possibly schema mismatch or history table issue.");
                    break; // Don't loop endlessly if migration itself fails
                }
            }
            else if (attempt == maxAttempts)
            {
                migrationLogger.LogError("[Migration] Giving up after {Max} attempts. Database exists but CanConnect is false. " +
                                       "Check 'sa' user permissions on PromotionsDb or run BaselineEfHistory_PromotionsDb.sql.", maxAttempts);
            }
            else
            {
                await Task.Delay(500);
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            migrationLogger.LogWarning(ex, "[Migration] SqlException {Number} on attempt {Attempt}/{Max}.", ex.Number, attempt, maxAttempts);
            if (attempt == maxAttempts)
                migrationLogger.LogError(ex, "[Migration] All attempts failed. Continuing startup without migration.");
            else
                await Task.Delay(3000);
        }
        catch (Exception ex)
        {
            migrationLogger.LogError(ex, "[Migration] Unexpected error. Continuing startup without migration.");
            break;
        }
    }
}
// -------------------------------

app.UseSerilogRequestLogging();
app.Use(async (context, next) => {
    context.Response.Headers["X-Promotions-Api-Version"] = "V4_NO_THROW";
    await next();
});
app.UseMiddleware<RequestLoggingMiddleware>(); 

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Additional dev-only settings if needed
}

// Global Exception Handler for clear API Errors
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        if (exception is ValidationException validationException)
        {
            logger.LogWarning("[ValidationError] Path: {Path}, Errors: {Errors}",
                exceptionHandlerPathFeature?.Path,
                string.Join("; ", validationException.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new 
            { 
                message = "Validation failed", 
                errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) 
            });
            await context.Response.WriteAsync(result);
        }
        else if (exception is InvalidPromotionReferenceException ipr)
        {
            logger.LogWarning(ipr, "[PromoReference] Path: {Path}, Message: {Message}",
                exceptionHandlerPathFeature?.Path, ipr.Message);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = ipr.Message });
            await context.Response.WriteAsync(result);
        }
        else if (exception is System.Collections.Generic.KeyNotFoundException)
        {
            logger.LogWarning("[KeyNotFound] Path: {Path}, Error: {Error}",
                exceptionHandlerPathFeature?.Path, exception.Message);

            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = exception.Message });
            await context.Response.WriteAsync(result);
        }
        else if (exception != null)
        {
            logger.LogError(exception, "[UnhandledException] Path: {Path}, Error: {Error}",
                exceptionHandlerPathFeature?.Path, exception.Message);

            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "An unexpected error occurred." });
            await context.Response.WriteAsync(result);
        }
    });
});

// app.UseHttpsRedirection(); // Removed to prevent Authorization header stripping during internal redirects in Development
app.UseAuthentication(); 
app.UseMiddleware<SessionValidationMiddleware>(); // Added Session Validation Middleware
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => true });

Log.Information(" Promotion API started successfully");

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information(" Promotion API is shutting down");
    Log.CloseAndFlush();
});

app.Run();
