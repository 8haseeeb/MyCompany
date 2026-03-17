using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using SSO.Application.Auth.Handlers;
using SSO.Application.Common;
using SSO.Application.Interfaces;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using SSO.Infrastructure.Security;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;
using SSO.Api.Security;


var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "SSO.Api");
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);

// OpenTelemetry: receive W3C traceparent from Gateway; export traces to Application Insights.
var otelConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrEmpty(otelConnectionString) && !otelConnectionString.StartsWith("REPLACE_"))
{
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = otelConnectionString);
        });
}


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
    builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });
else
    builder.Services.AddDistributedMemoryCache();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString!, name: "SSOIdentityDb", tags: new[] { "ready", "db" });

// DI registrations
builder.Services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddJwtAuthentication(builder.Configuration); // Added Auth service

// MediatR

builder.Services.AddAutoMapper(typeof(RegisterCommandHandler).Assembly);

builder.Services.AddValidatorsFromAssembly(typeof(RegisterCommandHandler).Assembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS from configuration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// --- DATABASE AUTO-MIGRATION WITH RETRY (DISABLED FOR COMPATIBILITY) ---
/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<IdentityDbContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            if (context.Database.IsSqlServer())
            {
                Console.WriteLine("Applying SSO database migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("SSO database migrated successfully.");

                break; // Success
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            retries--;
            Console.WriteLine($"Failed to connect to database. Retrying in 3 seconds... ({retries} attempts left). Error: {ex.Message}");
            if (retries == 0) throw;
            await Task.Delay(3000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
            throw;
        }
    }
}
*/
// -------------------------------


app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Additional dev-only settings if needed
}

// Global exception handler: map ValidationException to 400, others to 500
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var ex = feature?.Error;
        context.Response.ContentType = "application/json";
        if (ex is ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                message = "Validation failed",
                errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
            await context.Response.WriteAsync(result);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { message = "An unexpected error occurred." }));
        }
    });
});

app.UseMiddleware<RequestLoggingMiddleware>();

// app.UseHttpsRedirection(); // Removed to prevent Authorization header stripping during internal redirects in Development

app.UseAuthentication(); // Added Auth Middleware
app.UseMiddleware<SSO.Api.Middleware.SessionValidationMiddleware>(); // Added Session Middleware

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => true });
app.Run();
