using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SSO.Api.Security;

public static class JwtAuthExtensions
{
    // JWT uses short claim type "role"; MapInboundClaims=false keeps it — must match RoleClaimType for [Authorize(Roles)].
    private const string JwtRoleClaimType = "role";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var secret = configuration["JwtSettings:Secret"];
        const string placeholder = "REPLACE_VIA_ENV_OR_USER_SECRETS";
        if (string.IsNullOrEmpty(secret) || secret == placeholder)
            throw new InvalidOperationException("JWT Secret is not configured. Set JwtSettings:Secret via environment variable, User Secrets (dev), or Azure Key Vault (production).");

        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // For development
                options.SaveToken = true;
                options.MapInboundClaims = false; 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secret)
                    ),

                    NameClaimType = "unique_name",
                    RoleClaimType = JwtRoleClaimType
                };
            });

        services.AddAuthorization();

        return services;
    }
}
