using Microsoft.AspNetCore.Http;
using MyCompany.ApiGateway.Middlewares;
using MyCompany.ApiGateway.Routing;
using MyCompany.ApiGateway.Security;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// SERILOG CONFIGURATION
builder.Host.AddSerilogLogging(builder.Configuration, "ApiGateway");


//  JWT Authentication

builder.Services.AddJwtAuthentication(builder.Configuration);

//  HttpClient

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<DownstreamProxy>();


// Controllers

builder.Services.AddControllers();

var app = builder.Build();


//  STARTUP LOG

Log.Information(" API Gateway started successfully");

//  SERILOG REQUEST LOGGING

app.UseSerilogRequestLogging();


//  Global Middlewares

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>(); // Optional: logs request/response bodies


// Auth Middlewares

app.UseAuthentication();
app.UseAuthorization();

//  Catch-All Reverse Proxy

app.Map("/{**catch-all}", async context =>
{
    var path = context.Request.Path.Value?.ToLower();

    // ❌ Block Swagger
    if (path != null && path.StartsWith("/swagger"))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    // Enforce Authentication
    if (context.User?.Identity?.IsAuthenticated != true)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    // Resolve downstream service
    var baseUrl = RouteResolver.Resolve(context);
    var proxy = context.RequestServices.GetRequiredService<DownstreamProxy>();

    await proxy.ProxyAsync(context, $"{baseUrl}{context.Request.Path}");
});


//  SHUTDOWN LOG

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("🛑 API Gateway is shutting down");
    Log.CloseAndFlush();
});

app.Run();
