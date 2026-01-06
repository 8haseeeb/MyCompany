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
builder.Services.AddHttpClient<DownstreamProxy>();



builder.Services.AddControllers();

var app = builder.Build();



Log.Information(" API Gateway started successfully");


app.UseSerilogRequestLogging();



app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>(); 


app.UseAuthentication();
app.UseAuthorization();


app.Map("/{**catch-all}", async context =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (path != null && path.StartsWith("/swagger"))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    if (context.User?.Identity?.IsAuthenticated != true)
    {
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
