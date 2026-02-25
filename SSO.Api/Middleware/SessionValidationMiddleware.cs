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

                context.Response.Headers.Append("X-Session-Token", sessionIdClaim ?? "MISSING");

                if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    
                    if (user != null)
                    {
                        var dbSessionId = user.CurrentSessionId;
                        context.Response.Headers.Append("X-Session-DB", dbSessionId ?? "NULL");

                        if (dbSessionId != sessionIdClaim)
                        {
                            context.Response.Headers.Append("X-Session-Status", "MISMATCH");
                            _logger.LogWarning("[SessionCheck-SSO] MISMATCH! Logging out User: {UserId}.", userId);
                            
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
            else
            {
                // Optional: Log if we are hitting an endpoint that requires auth but user isn't authenticated yet
                // _logger.LogInformation("[SessionCheck-SSO] User not authenticated for {Path}", context.Request.Path);
            }

            await _next(context);
        }
    }
}
