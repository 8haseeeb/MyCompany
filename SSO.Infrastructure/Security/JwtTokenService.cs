using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SSO.Infrastructure.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user, string sessionId)
    {
        try
        {
            var secret = _config["JwtSettings:Secret"];
            if (string.IsNullOrWhiteSpace(secret) || secret.StartsWith("REPLACE_", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "JwtSettings:Secret is missing or not configured. Set it in appsettings.Development.json (Development) or environment variable JwtSettings__Secret.");

            var keyBytes = Encoding.UTF8.GetBytes(secret);
            if (keyBytes.Length < 32)
                throw new InvalidOperationException(
                    "JwtSettings:Secret must be at least 32 bytes (UTF-8) for HS256. Use a longer random secret in production.");

            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException(
                    "JwtSettings:Issuer and JwtSettings:Audience must be non-empty (see appsettings.json).");

            var role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role.Trim();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim("role", role),
                new Claim("SessionId", sessionId)
            };

            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to sign JWT. Verify JwtSettings:Secret, Issuer, and Audience.", ex);
        }
    }
}
