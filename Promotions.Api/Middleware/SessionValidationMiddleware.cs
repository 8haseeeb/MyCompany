using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Promotions.Infrastructure.Persistence.External;

namespace Promotions.Api.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, SsoDbContext ssoDbContext)
        {
            // Always stamp that this middleware ran
            context.Response.Headers["X-Middleware-Reached"] = "SESSION_VALIDATOR_V3";

            // Skip session check for health and login endpoints
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/api/health") || path.Contains("/api/auth"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                // Not authenticated — let authorization middleware handle it
                await _next(context);
                return;
            }

            // --- EXTRACT CLAIMS ---
            var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
            var subClaim = context.User.FindFirst("sub")?.Value;
            var nameIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdRaw = subClaim ?? nameIdClaim;

            context.Response.Headers["X-Session-Token"] = sessionIdClaim ?? "MISSING";
            context.Response.Headers["X-Session-UserClaim"] = userIdRaw ?? "MISSING";

            // If no session claim in token — block immediately
            if (string.IsNullOrEmpty(sessionIdClaim))
            {
                _logger.LogWarning("[SessionCheck-Promo] No SessionId claim in token. Blocking.");
                context.Response.Headers["X-Session-Status"] = "NO_SESSION_CLAIM";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"Invalid token: missing session.\"}");
                return;
            }

            if (string.IsNullOrEmpty(userIdRaw) || !int.TryParse(userIdRaw, out int userId))
            {
                _logger.LogWarning("[SessionCheck-Promo] No valid UserId claim. Blocking.");
                context.Response.Headers["X-Session-Status"] = "NO_USER_CLAIM";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"Invalid token: missing user id.\"}");
                return;
            }

            // --- DB CHECK (fail-safe: deny if DB unreachable) ---
            try
            {
                var user = await ssoDbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    context.Response.Headers["X-Session-Status"] = "USER_NOT_FOUND";
                    _logger.LogWarning("[SessionCheck-Promo] User {UserId} not found in SSO DB.", userId);
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\": \"User not found.\"}");
                    return;
                }

                var dbSessionId = user.CurrentSessionId;
                context.Response.Headers["X-Session-DB"] = dbSessionId ?? "NULL";

                if (dbSessionId != sessionIdClaim)
                {
                    context.Response.Headers["X-Session-Status"] = "MISMATCH";
                    _logger.LogWarning(
                        "[SessionCheck-Promo] MISMATCH! User={UserId}, Token={TokenSession}, DB={DbSession}",
                        userId, sessionIdClaim, dbSessionId);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                    return;
                }

                context.Response.Headers["X-Session-Status"] = "VALID";
                _logger.LogDebug("[SessionCheck-Promo] Session VALID for User={UserId}", userId);
            }
            catch (Exception ex)
            {
                // FAIL-SAFE: DB unreachable → block the request, do NOT let it through
                _logger.LogError(ex, "[SessionCheck-Promo] DB check failed for User={UserId}. Blocking request.", userId);
                context.Response.Headers["X-Session-Status"] = $"DB_ERROR:{ex.GetType().Name}";
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"Session validation failed. Please try again.\"}");
                return;  // <-- CRITICAL: do NOT call _next on error
            }

            // ✅ Only allow request if session is valid
            await _next(context);
        }
    }
}