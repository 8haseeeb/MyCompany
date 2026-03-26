using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Serilog;

namespace Promotions.Api.Security;

public static class JwtAuthExtensions
{
    /// <summary>
    /// JWT payloads use the short claim name <c>role</c> (see JwtSecurityTokenHandler outbound map for ClaimTypes.Role).
    /// With <see cref="JwtBearerOptions.MapInboundClaims"/> = false, that short name is preserved — it must match
    /// <see cref="TokenValidationParameters.RoleClaimType"/> or <c>[Authorize(Roles = "Admin")]</c> never sees roles.
    /// </summary>
    private const string JwtRoleClaimType = "role";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"];
        const string placeholder = "REPLACE_VIA_ENV_OR_USER_SECRETS";
        if (string.IsNullOrEmpty(secret) || secret == placeholder)
            throw new InvalidOperationException("JWT Secret is not configured. Set JwtSettings:Secret via environment variable, User Secrets (dev), or Azure Key Vault (production).");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secret!)
                    ),

                    NameClaimType = "unique_name",
                    RoleClaimType = JwtRoleClaimType
                };

                //  DIAGNOSTIC LOGGING
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var roles = context.Principal?.Claims
                            .Where(c => c.Type == JwtRoleClaimType || c.Type.EndsWith("/role", StringComparison.Ordinal))
                            .Select(c => c.Value)
                            .ToList();
                        Log.Information(
                            "Downstream JWT validated for {User}; role claims: {Roles}",
                            context.Principal?.Identity?.Name ?? "Anonymous",
                            roles is { Count: > 0 } ? string.Join(", ", roles) : "(none)");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Log.Error(context.Exception, "Downstream JWT Authentication Failed for {Path}", 
                            context.HttpContext.Request.Path);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Log.Warning("Downstream JWT Challenge triggered for {Path}. Error: {Error}, Description: {Desc}",
                            context.HttpContext.Request.Path, context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
