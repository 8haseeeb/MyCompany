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

var ssoAiConn = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(ssoAiConn) && !ssoAiConn.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddApplicationInsightsTelemetry(o => o.ConnectionString = ssoAiConn);

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


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
    builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnection; });
else
    builder.Services.AddDistributedMemoryCache();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString!, name: "SSOServiceDb", tags: new[] { "ready", "db" });

// DI registrations
builder.Services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserSessionTokenService, UserSessionTokenService>();

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

// --- DATABASE AUTO-MIGRATION WITH RETRY ---
// Fresh IdentityDbContext per attempt (same rationale as Promotions.Api — avoid broken connection after SqlException).
{
    const int maxAttempts = 12;
    var succeeded = false;

    for (var attempt = 1; attempt <= maxAttempts && !succeeded; attempt++)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        try
        {
            if (!context.Database.IsSqlServer())
                break;

            Console.WriteLine($"Applying SSO database migrations (attempt {attempt}/{maxAttempts})...");
            await context.Database.MigrateAsync();
            Console.WriteLine("SSO database migrated successfully.");
            succeeded = true;
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number is 2714 or 1913)
        {
            // 2714 = Object already exists (e.g. Users table)
            // 1913 = Index already exists
            Console.WriteLine($"[Migration] Schema objects already exist in SSOServiceDb (Error {ex.Number}). Verifying connectivity...");
            if (await context.Database.CanConnectAsync())
            {
                Console.WriteLine("Connection to SSOServiceDb is healthy. Assuming baseline schema is present. Proceeding...");
                succeeded = true; 
            }
            else
            {
                Console.WriteLine("[Migration] Cannot connect to SSOServiceDb even though objects reported to exist. Check permissions.");
                break;
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1801)
        {
            Console.WriteLine($"[Migration] SqlException 1801 (database already exists). Verifying if reachable...");
            if (await context.Database.CanConnectAsync())
            {
                Console.WriteLine("SSOServiceDb exists and is reachable. Proceeding...");
                succeeded = true;
            }
            else if (attempt == maxAttempts)
            {
                Console.WriteLine("[Migration] Giving up on 1801. Database exists but CanConnect is false.");
            }
            else
            {
                await Task.Delay(500);
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            Console.WriteLine($"[Migration] SqlException {ex.Number} on attempt {attempt}/{maxAttempts}: {ex.Message}");
            if (attempt == maxAttempts)
                Console.WriteLine("All attempts exhausted. Continuing startup without migration.");
            else
                await Task.Delay(3000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while migrating the database: {ex.Message}. Continuing startup...");
            break;
        }
    }
}
// -------------------------------

// Early so failures in later middleware (including controllers) return JSON bodies reliably.
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

app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Additional dev-only settings if needed
}

app.UseMiddleware<RequestLoggingMiddleware>();

// app.UseHttpsRedirection(); // Removed to prevent Authorization header stripping during internal redirects in Development

app.UseAuthentication(); // Added Auth Middleware
app.UseMiddleware<SSO.Api.Middleware.SessionValidationMiddleware>(); // Added Session Middleware

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { Predicate = _ => true });
app.Run();
