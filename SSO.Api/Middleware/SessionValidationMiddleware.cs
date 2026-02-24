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
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
                
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                               ?? context.User.FindFirst("sub")?.Value;

                _logger.LogInformation("[SessionCheck] User authenticated. UserIdClaim: {UserIdClaim}, SessionIdClaim: {SessionIdClaim}", 
                    userIdClaim ?? "NULL", sessionIdClaim ?? "NULL");

                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    
                    if (user != null)
                    {
                        _logger.LogInformation("[SessionCheck] DB User found. DB Session: {DbSession}, Token Session: {TokenSession}", 
                            user.CurrentSessionId ?? "NULL", sessionIdClaim);

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
