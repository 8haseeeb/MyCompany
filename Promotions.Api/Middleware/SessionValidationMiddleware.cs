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

                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await ssoDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                    
                    if (user != null)
                    {
                        // Only validate session if the DB actually has a session ID stored.
                        // Since we are ignoring this column in SsoDbContext to avoid schema errors,
                        // it will always be null here unless the schema is updated.
                        if (!string.IsNullOrEmpty(user.CurrentSessionId) && user.CurrentSessionId != sessionIdClaim)
                        {
                            _logger.LogWarning("Session Mismatch! User: {UserId}. TokenSession: {TokenSession}, DBSession: {DbSession}", userId, sessionIdClaim, user.CurrentSessionId);
                            
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                            return; 
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
