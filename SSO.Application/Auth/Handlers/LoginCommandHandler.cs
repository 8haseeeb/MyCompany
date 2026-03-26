using MediatR;
using SSO.Application.Auth;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;

using Serilog;

namespace SSO.Application.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
{
    private readonly IUserRepository _users;
    private readonly IUserSessionTokenService _sessionTokens;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IUserRepository users,
        IUserSessionTokenService sessionTokens,
        IPasswordHasher passwordHasher)
    {
        _users = users;
        _sessionTokens = sessionTokens;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = AuthEmailNormalizer.Normalize(request.Email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            Log.Warning("Login failed: empty email after normalization");
            throw new InvalidCredentialsException();
        }

        var user = await _users.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user == null)
        {
            Log.Warning("Login failed for unknown user: {Email}", normalizedEmail);
            throw new InvalidCredentialsException();
        }

        if (!_passwordHasher.LooksLikeBcryptHash(user.PasswordHash))
        {
            Log.Warning("Login failed for user {Email}: password hash missing or not bcrypt-shaped", user.Email);
            throw new InvalidCredentialsException();
        }

        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            Log.Warning("Login failed for user: {Email}. Reason: Invalid password", user.Email);
            throw new InvalidCredentialsException();
        }

        LoginResultDto result;
        try
        {
            result = await _sessionTokens.IssueSessionAndTokensAsync(user, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login persistence/token failed for {Email}", user.Email);
            throw new InvalidOperationException("Login could not be completed. Check JWT configuration and database.", ex);
        }

        Log.Information(
            "User {Email} logged in successfully. UserId: {UserId} Role: {Role}",
            user.Email,
            user.Id,
            result.Role);
        return result;
    }
}
