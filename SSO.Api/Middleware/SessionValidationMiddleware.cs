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
                var subClaim = context.User.FindFirst("sub")?.Value;
                var nameIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var userIdClaim = subClaim ?? nameIdClaim;

                context.Response.Headers.Append("X-Session-Token", sessionIdClaim ?? "MISSING");
                context.Response.Headers.Append("X-Session-UserClaim", userIdClaim ?? "MISSING");

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
                            _logger.LogWarning("[SessionCheck-SSO] MISMATCH! User: {UserId}, DB: {DbSession}, Token: {TokenSession}", userId, dbSessionId, sessionIdClaim);
                            
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                            return; 
                        }
                        context.Response.Headers.Append("X-Session-Status", "VALID");
                    }
                    else
                    {
                        context.Response.Headers.Append("X-Session-Status", "USER_NOT_FOUND_IN_DB");
                    }
                }
                else
                {
                    context.Response.Headers.Append("X-Session-Status", $"CLAIMS_INVALID_OR_MISSING_SUB_{subClaim ?? "NULL"}_ID_{nameIdClaim ?? "NULL"}");
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
