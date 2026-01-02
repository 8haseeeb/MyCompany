using MediatR;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog; // RequestLoggingMiddleware namespace
using Promotions.Api.Controllers;
using Promotions.Api.Security;
using Promotions.Application;
using Promotions.Application.Common;
using Promotions.Application.Participant.Interfaces;
using Promotions.Infrastructure;
using Promotions.Infrastructure.Participant;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


//  SERILOG SETUP

builder.Host.AddSerilogLogging(builder.Configuration, "Promotions.Api");


// Add services to the container

builder.Services.AddControllers();

// ✅ Swagger with JWT Authorization
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token"
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


// ✅ JWT Authentication

builder.Services.AddJwtAuthentication(builder.Configuration);


// ✅ MediatR

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


// ✅ Infrastructure / DB

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionString!);


var app = builder.Build();


// 🔥 Serilog Request Logging

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>(); // logs request/response bodies


// ✅ Development tools

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ✅ Middleware

app.UseHttpsRedirection();
app.UseAuthentication(); // JWT authentication
app.UseAuthorization();

app.MapControllers();


// 🚀 Startup log

Log.Information("🚀 Promotion API started successfully");

// 🛑 Shutdown log

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("🛑 Promotion API is shutting down");
    Log.CloseAndFlush();
});

app.Run();
