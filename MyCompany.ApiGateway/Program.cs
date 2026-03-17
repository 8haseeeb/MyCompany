using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using MyCompany.ApiGateway.Middlewares;
using MyCompany.ApiGateway.Routing;
using MyCompany.ApiGateway.Security;
using MyCompany.ApiGateway.Resilience;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "ApiGateway");
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);

// OpenTelemetry: W3C trace context flows Gateway → downstream (SSO/Promotions). Export to Application Insights.
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



builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddHttpClient();
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddHttpClient<DownstreamProxy>()
    .AddPolicyHandler(RetryPolicies.GetRetryPolicy())
    .AddPolicyHandler(CircuitBreakerPolicies.GetCircuitBreakerPolicy())
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        // Only disable TLS certificate validation in Development (e.g. self-signed downstream); production uses default validation.
        ServerCertificateCustomValidationCallback = isDevelopment ? (_, __, ___, ____) => true : null
    });

// Named client for gateway health checks: same TLS rules (strict in Production, relaxed in Development).
builder.Services.AddHttpClient("HealthCheck")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = isDevelopment ? (_, __, ___, ____) => true : null
    });



builder.Services.AddControllers();

// CORS from configuration (no hardcoded production origins)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithExposedHeaders("X-Session-Status", "X-Session-DB", "X-Session-Token", "X-Session-Middleware", "X-Middleware-Reached", "X-Correlation-ID");
        });
});

// Rate limiting (built-in .NET 8): fixed window per IP at gateway (100 req/min per IP)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { Window = TimeSpan.FromMinutes(1), PermitLimit = 100 }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Middleware

// 1. Buffering (Important for Proxying)
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// 2. CORS (Must be early)
app.UseCors("AllowReactApp");

// 2b. Rate limiting (before heavy middleware)
app.UseRateLimiter();

// 2c. Security headers (X-Content-Type-Options, X-Frame-Options, HSTS when HTTPS)
app.UseMiddleware<SecurityHeadersMiddleware>();

// 3. Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 4. Logging & Metadata
app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>(); 
app.UseMiddleware<CorrelationIdMiddleware>();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Map("/{**catch-all}", async context =>
{
    var path = context.Request.Path.Value?.ToLower();
    context.Response.Headers["X-Proxy-Gateway"] = "ACTIVE";

    if (path != null && path.StartsWith("/swagger"))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    var isAuthPath = path != null && (path.Contains("/api/auth") || path.Contains("/api/gateway/health"));

    if (!isAuthPath && context.User?.Identity?.IsAuthenticated != true)
    {
        Log.Warning("Blocking unauthorized request to downstream {Path}. IsAuthenticated: {IsAuth}, User: {User}", 
            path, 
            context.User?.Identity?.IsAuthenticated,
            context.User?.Identity?.Name ?? "Anonymous");

        context.Response.Headers["X-Session-Status"] = "GATEWAY_UNAUTH";
        context.Response.Headers["X-Session-Middleware"] = "GATEWAY_BLOCKED";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"message\": \"Unauthorized. Please log in.\"}");
        return;
    }

    // Backward compatibility: rewrite /api/... to /api/v1/... when no version segment present
    var downstreamPath = RouteResolver.RewriteToVersionedPath(path ?? "");
    var baseUrl = RouteResolver.ResolveByPath(downstreamPath);
    var proxy = context.RequestServices.GetRequiredService<DownstreamProxy>();

    await proxy.ProxyAsync(context, $"{baseUrl}{downstreamPath}");
});



app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information(" API Gateway is shutting down");
    Log.CloseAndFlush();
});

app.Run();
