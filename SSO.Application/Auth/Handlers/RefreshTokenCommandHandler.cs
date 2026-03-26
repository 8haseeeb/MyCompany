using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;


namespace SSO.Application.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto>
{
    private readonly IIdentityDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IIdentityDbContext context,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<RefreshTokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[RefreshToken] Attempting token refresh.");

        // Search for user with this refresh token
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("[RefreshToken] Invalid or expired refresh token.");
            throw new Exception("Invalid or expired refresh token");
        }

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

        var role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role.Trim();
        _logger.LogInformation("[RefreshToken] Token refreshed successfully. UserId: {UserId} Role: {Role}", user.Id, role);
        return new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Role = role
        };
    }
}
