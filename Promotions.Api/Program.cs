using MediatR;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog; 
using Promotions.Api.Controllers;
using Promotions.Api.Security;
using Promotions.Application;
using Promotions.Application.Common;
using Promotions.Application.Participant.Interfaces;
using Promotions.Infrastructure;
using Promotions.Infrastructure.Participant;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "Promotions.Api");
builder.Services.AddControllers();
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionString!);


var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<RequestLoggingMiddleware>(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

Log.Information(" Promotion API started successfully");

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information(" Promotion API is shutting down");
    Log.CloseAndFlush();
});

app.Run();
