using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;
using SSO.Domain.RefreshTokens;

namespace SSO.Application.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(IIdentityDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<RefreshTokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Get token with user
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User) // User navigation property must exist
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Invalid or expired refresh token");

        // Revoke old refresh token
        tokenEntity.IsRevoked = true;

        // Create new refresh token
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = tokenEntity.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate new access token
        var newAccessToken = _jwtTokenService.GenerateToken(tokenEntity.User);

        return new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}
