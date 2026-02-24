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

                _logger.LogInformation("[SessionCheck] User authenticated. UserIdClaim: {UserIdClaim}, SessionIdClaim: {SessionIdClaim}", 
                    userIdClaim ?? "NULL", sessionIdClaim ?? "NULL");

                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await ssoDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                    
                    if (user != null)
                    {
                        _logger.LogInformation("[SessionCheck] DB User found. DB Session: {DbSession}, Token Session: {TokenSession}", 
                            user.CurrentSessionId ?? "NULL", sessionIdClaim);

                        // Validate session: if the DB has a different session ID than the token, it's an old session.
                        if (user.CurrentSessionId != sessionIdClaim)
                        {
                            _logger.LogWarning("Session Mismatch! User: {UserId}. TokenSession: {TokenSession}, DBSession: {DbSession}", userId, sessionIdClaim, user.CurrentSessionId);
                            
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                            return; 
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[SessionCheck] User {UserId} NOT found in SSO database", userId);
                    }
                }
                else
                {
                     _logger.LogWarning("[SessionCheck] Skipping validation. SessionIdClaim or UserIdClaim is missing or invalid.");
                }
            }

            await _next(context);
        }
    }
}
