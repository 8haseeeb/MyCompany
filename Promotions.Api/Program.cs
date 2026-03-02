using MediatR;
using Microsoft.EntityFrameworkCore;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog; 
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

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "Promotions.Api");
builder.Services.AddLoggingLevelSwitch(); // Enable dynamic logging level control
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);
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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<PromotionsDbContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            if (context.Database.IsSqlServer())
            {
                logger.LogInformation("Applying Promotions database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Promotions database migrated successfully.");

                break; // Success
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            retries--;
            logger.LogWarning(ex, "[Migration] SqlException (Error {Number}). Retries left: {Retries}. Will retry...", ex.Number, retries);
            if (retries == 0)
            {
                // Do NOT crash the container — migrations might already be applied
                logger.LogError(ex, "[Migration] All retries exhausted. Continuing startup without migration.");
                break;
            }
            await Task.Delay(3000);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database. Continuing startup...");
            break; // Don't crash the container on migration errors — app can still start
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

Log.Information(" Promotion API started successfully");

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information(" Promotion API is shutting down");
    Log.CloseAndFlush();
});

app.Run();
