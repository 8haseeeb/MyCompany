using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Handlers;
using SSO.Application.Interfaces;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using SSO.Infrastructure.Security;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "SSO.Api");

// ----------------------------
// Database
// ----------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

// DI registrations
builder.Services.AddScoped<IIdentityDbContext, IdentityDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ----------------------------
// MediatR
// ----------------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));

// ----------------------------
// Controllers & Swagger
// ----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// CORS
// ----------------------------
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

// ----------------------------
// Build App
// ----------------------------
var app = builder.Build();

// ----------------------------
// Middleware
// ----------------------------
app.UseCors("AllowReactApp");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
