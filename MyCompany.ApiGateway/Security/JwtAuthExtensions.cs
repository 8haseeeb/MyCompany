using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Security.Claims;

namespace MyCompany.ApiGateway.Security;

public static class JwtAuthExtensions
{
    // JWT uses short claim type "role"; MapInboundClaims=false keeps it — must match RoleClaimType for role checks.
    private const string JwtRoleClaimType = "role";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var secret = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        const string placeholder = "REPLACE_VIA_ENV_OR_USER_SECRETS";
        if (string.IsNullOrEmpty(secret) || secret == placeholder)
            throw new InvalidOperationException("JWT Secret is not configured. Set JwtSettings:Secret via environment variable, User Secrets (dev), or Azure Key Vault (production).");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.MapInboundClaims = false; // Prevent mapping to SOAP namespaces

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
                    ),

                    NameClaimType = "unique_name",
                    RoleClaimType = JwtRoleClaimType
                };

                //  JWT EVENTS + SERILOG
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var roles = context.Principal?.Claims
                            .Where(c => c.Type == JwtRoleClaimType || c.Type.EndsWith("/role", StringComparison.Ordinal))
                            .Select(c => c.Value)
                            .ToList();
                        Log.Information(
                            "JWT validated for {User}; role claims: {Roles}",
                            context.Principal?.Identity?.Name ?? "Anonymous",
                            roles is { Count: > 0 } ? string.Join(", ", roles) : "(none)");

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
