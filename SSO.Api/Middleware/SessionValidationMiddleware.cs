using Microsoft.AspNetCore.Http;
using SSO.Application.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SSO.Api.Middleware
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

        public async Task InvokeAsync(HttpContext context, IIdentityDbContext dbContext)
        {
            // Skip session check only for health. Login has no/invalid token so passes early; refresh MUST be validated.
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.Contains("/health"))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
            var subClaim = context.User.FindFirst("sub")?.Value;
            var nameIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userIdClaim = subClaim ?? nameIdClaim;

            context.Response.Headers["X-Session-Token"] = sessionIdClaim ?? "MISSING";
            context.Response.Headers["X-Session-UserClaim"] = userIdClaim ?? "MISSING";

            if (string.IsNullOrEmpty(sessionIdClaim) || string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                context.Response.Headers["X-Session-Status"] = $"CLAIMS_INVALID_OR_MISSING";
                context.Response.Headers["X-Session-DB"] = "N/A";
                _logger.LogWarning("[SessionCheck-SSO] Invalid or missing claims.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"Invalid token: missing session or user claim.\"}");
                return;
            }

            try
            {
                var user = await dbContext.Users.FindAsync(userId);

                if (user == null)
                {
                    context.Response.Headers["X-Session-Status"] = "USER_NOT_FOUND";
                    context.Response.Headers["X-Session-DB"] = "NULL";
                    _logger.LogWarning("[SessionCheck-SSO] User {UserId} not found in DB.", userId);
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
                    _logger.LogWarning("[SessionCheck-SSO] MISMATCH! User: {UserId}, DB: {DbSession}, Token: {TokenSession}", userId, dbSessionId, sessionIdClaim);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                    return;
                }

                context.Response.Headers["X-Session-Status"] = "VALID";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SessionCheck-SSO] DB check failed for User={UserId}. Blocking request.", userId);
                context.Response.Headers["X-Session-Status"] = "DB_ERROR";
                context.Response.Headers["X-Session-DB"] = "ERROR";
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"Session validation failed. Please try again.\"}");
                return;
            }

            await _next(context);
        }
    }
}
