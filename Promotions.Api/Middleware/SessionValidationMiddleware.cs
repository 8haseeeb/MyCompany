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
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                               ?? context.User.FindFirst("sub")?.Value;

                context.Response.Headers.Append("X-Session-Token", sessionIdClaim ?? "MISSING");
                
                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await ssoDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                    
                    if (user != null)
                    {
                        var dbSessionId = user.CurrentSessionId;
                        context.Response.Headers.Append("X-Session-DB", dbSessionId ?? "NULL");

                        // Validate session: if the DB has a different session ID than the token, it's an old session.
                        if (dbSessionId != sessionIdClaim)
                        {
                            context.Response.Headers.Append("X-Session-Status", "MISMATCH");
                            _logger.LogWarning("[SessionCheck-Promo] MISMATCH! Logging out User: {UserId}", userId);
                            
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                            return; 
                        }
                        context.Response.Headers.Append("X-Session-Status", "VALID");
                    }
                    else
                    {
                        context.Response.Headers.Append("X-Session-Status", "USER_NOT_FOUND");
                    }
                }
                else
                {
                     context.Response.Headers.Append("X-Session-Status", "CLAIMS_MISSING");
                }
            }

            await _next(context);
        }
    }
}
