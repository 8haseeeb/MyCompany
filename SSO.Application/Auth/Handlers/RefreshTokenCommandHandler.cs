using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;


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
        // Search for user with this refresh token
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new Exception("Invalid or expired refresh token");

        // Generate new token
        var newRefreshToken = Guid.NewGuid().ToString();
        var newExpiry = DateTime.UtcNow.AddDays(30);

        // Update user
        user.UpdateRefreshToken(newRefreshToken, newExpiry);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate new access token
        var sessionId = user.CurrentSessionId ?? Guid.NewGuid().ToString(); 
        var newAccessToken = _jwtTokenService.GenerateToken(user, sessionId);

        return new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

}
