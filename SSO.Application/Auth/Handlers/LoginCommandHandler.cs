using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;
using SSO.Domain.RefreshTokens;
using Serilog;

namespace SSO.Application.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IIdentityDbContext _context;

    public LoginCommandHandler(
        IUserRepository users,
        IJwtTokenService jwt,
        IPasswordHasher passwordHasher,
        IIdentityDbContext context)
    {
        _users = users;
        _jwt = jwt;
        _passwordHasher = passwordHasher;
        _context = context;
    }

    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1️⃣ Get user by email
        var user = await _users.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            Log.Warning("Login failed for unknown user: {Email}", request.Email);
            throw new Exception("Invalid credentials");
        }

        // 2️⃣ Verify password
        if (!_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            Log.Warning("Login failed for user: {Email}. Reason: Invalid password", user.Email);
            throw new Exception("Invalid credentials");
        }

        // 3️⃣ Generate Access Token
        var accessToken = _jwt.GenerateToken(user);

        // 4️⃣ Generate Refresh Token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        };
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 5️⃣ Return both tokens
        Log.Information("User {Email} logged in successfully. UserId: {UserId}", user.Email, user.Id);

        return new LoginResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}
