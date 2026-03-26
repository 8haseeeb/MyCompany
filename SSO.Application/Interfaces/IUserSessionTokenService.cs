using SSO.Application.Dtos;
using SSO.Domain.Users;

namespace SSO.Application.Interfaces;

/// <summary>
/// Issues JWT + refresh token and persists session fields on the user (same path as login).
/// </summary>
public interface IUserSessionTokenService
{
    Task<LoginResultDto> IssueSessionAndTokensAsync(User user, CancellationToken cancellationToken);
}
