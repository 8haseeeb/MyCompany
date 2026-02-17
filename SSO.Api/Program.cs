using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Handlers;
using SSO.Application.Interfaces;
using SSO.Infrastructure.Persistence;
using SSO.Infrastructure.Repositories;
using SSO.Infrastructure.Security;
using MyCompany.Common.Logging;
using MyCompany.Common.Logging.Serilog;

using SSO.Api.Security;

// ... (keep existing usings)

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging(builder.Configuration, "SSO.Api");


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

// DI registrations
builder.Services.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddJwtAuthentication(builder.Configuration); // Added Auth service

// MediatR

builder.Services.AddAutoMapper(typeof(RegisterCommandHandler).Assembly);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// --- DATABASE AUTO-MIGRATION ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IdentityDbContext>();
        if (context.Database.IsSqlServer())
        {
            // Note: Use Serilog if configured, otherwise falls back to default logging
            Console.WriteLine("Applying SSO database migrations...");
            await context.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}
// -------------------------------

app.UseCors("AllowReactApp");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();

// app.UseHttpsRedirection(); // Removed to prevent Authorization header stripping during internal redirects in Development

app.UseAuthentication(); // Added Auth Middleware
app.UseMiddleware<SSO.Api.Middleware.SessionValidationMiddleware>(); // Added Session Middleware

app.UseAuthorization();
app.MapControllers();
app.Run();
