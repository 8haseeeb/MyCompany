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

                _logger.LogInformation("[SessionCheck-SSO] Authenticated. UserId: {UserIdClaim}, SessionId: {SessionIdClaim}", 
                    userIdClaim ?? "NULL", sessionIdClaim ?? "NULL");

                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    
                    if (user != null)
                    {
                        var dbSessionId = user.CurrentSessionId;
                        _logger.LogInformation("[SessionCheck-SSO] DB Match. User: {UserId}, DB Session: {DbSession}, Token Session: {TokenSession}", 
                            userId, dbSessionId ?? "NULL", sessionIdClaim);

                        if (dbSessionId != sessionIdClaim)
                        {
                            _logger.LogWarning("[SessionCheck-SSO] MISMATCH! Logging out User: {UserId}.", userId);
                            
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                            return; 
                        }
                    }
                    else
                    {
                        _logger.LogWarning("[SessionCheck-SSO] User {UserId} NOT found in DB.", userId);
                    }
                }
                else
                {
                    _logger.LogWarning("[SessionCheck-SSO] Skipping. Missing Claims. UserId: {UserIdClaim}, SessionId: {SessionIdClaim}", 
                        userIdClaim ?? "NULL", sessionIdClaim ?? "NULL");
                }
            }
            else
            {
                // Optional: Log if we are hitting an endpoint that requires auth but user isn't authenticated yet
                // _logger.LogInformation("[SessionCheck-SSO] User not authenticated for {Path}", context.Request.Path);
            }

            await _next(context);
        }
    }
}
