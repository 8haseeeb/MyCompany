using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace MyCompany.ApiGateway.Security;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secret = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (string.IsNullOrEmpty(secret))
            throw new Exception("JWT Secret is missing in Gateway configuration");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secret)
                    )
                };

                // 🔥 JWT EVENTS + SERILOG
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        Log.Information(
                            "JWT validated for {User}",
                            context.Principal?.Identity?.Name ?? "Anonymous"
                        );

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning(
                            context.Exception,
                            "JWT authentication failed"
                        );

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        Log.Warning(
                            "JWT challenge triggered. Error: {Error}, Description: {Description}",
                            context.Error,
                            context.ErrorDescription
                        );

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
