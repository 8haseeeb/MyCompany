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

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "Promotions.Api");
builder.Services.AddControllers();
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

builder.Services.AddAutoMapper(typeof(AssemblyMarker).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var ssoConnectionString = builder.Configuration.GetConnectionString("SsoConnection");

builder.Services.AddInfrastructure(connectionString!);

// Add SsoDbContext for Session Validation
builder.Services.AddDbContext<SsoDbContext>(options =>
    options.UseSqlServer(ssoConnectionString));


var app = builder.Build();

app.UseSerilogRequestLogging();
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

        if (exception is System.Collections.Generic.KeyNotFoundException)
        {
            context.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = exception.Message });
            await context.Response.WriteAsync(result);
        }
    });
});

app.UseHttpsRedirection();
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
