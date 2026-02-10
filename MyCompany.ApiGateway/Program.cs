 using Microsoft.AspNetCore.Http;
using MyCompany.ApiGateway.Middlewares;
using MyCompany.ApiGateway.Routing;
using MyCompany.ApiGateway.Security;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "ApiGateway");



builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddHttpClient();
builder.Services.AddHttpClient<DownstreamProxy>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });



builder.Services.AddControllers();

// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
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

    if (path != null && path.StartsWith("/swagger"))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    var isAuthPath = path != null && path.Contains("/api/auth");

    if (!isAuthPath && context.User?.Identity?.IsAuthenticated != true)
    {
        Log.Warning("Blocking unauthorized request to downstream {Path}. IsAuthenticated: {IsAuth}, User: {User}", 
            path, 
            context.User?.Identity?.IsAuthenticated,
            context.User?.Identity?.Name ?? "Anonymous");

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    var baseUrl = RouteResolver.Resolve(context);
    var proxy = context.RequestServices.GetRequiredService<DownstreamProxy>();

    await proxy.ProxyAsync(context, $"{baseUrl}{context.Request.Path}");
});



app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information(" API Gateway is shutting down");
    Log.CloseAndFlush();
});

app.Run();
