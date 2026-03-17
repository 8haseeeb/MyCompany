using MediatR;

namespace SSO.Application.Auth.Commands;

/// <summary>
/// Clears the user's session and refresh token in the database so the token is no longer valid.
/// Caller (API) should also remove the session validation cache key so Promotions rejects the session immediately.
/// </summary>
public record LogoutCommand(int UserId) : IRequest<Unit>;
