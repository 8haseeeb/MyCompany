using Microsoft.EntityFrameworkCore;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using SSO.Infrastructure.Persistence;

namespace SSO.Infrastructure.Security;

public class UserSessionTokenService : IUserSessionTokenService
{
    private readonly IdentityDbContext _context;
    private readonly IJwtTokenService _jwt;

    public UserSessionTokenService(IdentityDbContext context, IJwtTokenService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<LoginResultDto> IssueSessionAndTokensAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            var newSessionId = Guid.NewGuid().ToString();
            string accessToken;
            try
            {
                accessToken = _jwt.GenerateToken(user, newSessionId);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "JWT creation failed. Ensure JwtSettings:Secret is at least 32 UTF-8 bytes and Issuer/Audience are set.",
                    ex);
            }

            user.UpdateSession(newSessionId);
            var refreshTokenString = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddDays(30);
            user.UpdateRefreshToken(refreshTokenString, expiry);

            var entry = _context.Entry(user);
            if (entry.State == EntityState.Detached)
                _context.Users.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role.Trim();
            return new LoginResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                UserName = user.UserName,
                Role = role
            };
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Could not save session after issuing tokens. Check database connectivity and schema.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Token or session persistence failed. Check JwtSettings and database.", ex);
        }
    }
}
