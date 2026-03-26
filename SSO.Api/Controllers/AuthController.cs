using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SSO.Application.Auth;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;

namespace SSO.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    /// <summary>Must match Promotions.Api.Middleware.SessionValidationMiddleware cache key prefix.</summary>
    private const string SessionValidationCacheKeyPrefix = "SessionValidation:";

    private readonly IMediator _mediator;
    private readonly IDistributedCache _cache;

    public AuthController(IMediator mediator, IDistributedCache cache)
    {
        _mediator = mediator;
        _cache = cache;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }
        catch (InvalidOperationException ex)
        {
            // Misconfiguration (e.g. JWT secret) — 503 so it is not confused with bad password
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
        }
        catch (Exception ex) when (ex.Message == "Invalid credentials")
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _mediator.Send(new RegisterCommand(dto.UserName, dto.Email, dto.Password));
            return StatusCode(StatusCodes.Status201Created, result);
        }
        catch (DuplicateUserException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(dto.RefreshToken));
        return Ok(result);
    }

    /// <summary>
    /// Logout: clears session and refresh token in DB and invalidates session cache (Redis) so Promotions rejects the token immediately.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized(new { message = "Invalid token." });

        await _mediator.Send(new LogoutCommand(userId));

        // Invalidate session cache so Promotions (and any other consumer) rejects this session immediately.
        await _cache.RemoveAsync(SessionValidationCacheKeyPrefix + userId);

        return Ok(new { message = "Logged out successfully." });
    }
}
