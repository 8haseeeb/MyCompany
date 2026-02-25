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
            context.Response.Headers["X-Middleware-Reached"] = "PROMOTIONS_VAL_ACTIVE";
            
            try 
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
                    var subClaim = context.User.FindFirst("sub")?.Value;
                    var nameIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    
                    var userIdClaim = subClaim ?? nameIdClaim;

                    context.Response.Headers["X-Session-Token"] = sessionIdClaim ?? "MISSING";
                    context.Response.Headers["X-Session-UserClaim"] = userIdClaim ?? "MISSING";
                    
                    if (!string.IsNullOrEmpty(sessionIdClaim) && !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                    {
                        var user = await ssoDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                        
                        if (user != null)
                        {
                            var dbSessionId = user.CurrentSessionId;
                            context.Response.Headers["X-Session-DB"] = dbSessionId ?? "NULL";

                            if (dbSessionId != sessionIdClaim)
                            {
                                context.Response.Headers["X-Session-Status"] = "MISMATCH";
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                await context.Response.WriteAsync("{\"message\": \"Session expired. You are logged in on another device.\"}");
                                return; 
                            }
                            context.Response.Headers["X-Session-Status"] = "VALID";
                        }
                        else
                        {
                            context.Response.Headers["X-Session-Status"] = "USER_NOT_FOUND_IN_SSO_DB";
                        }
                    }
                    else
                    {
                        context.Response.Headers["X-Session-Status"] = $"CLAIMS_INVALID_OR_MISSING_SUB_{subClaim ?? "NULL"}";
                    }
                }
                else 
                {
                    context.Response.Headers["X-Session-Status"] = "NOT_AUTHENTICATED_IN_DOWNSTREAM";
                }
            }
            catch (Exception ex)
            {
                context.Response.Headers["X-Middleware-Error"] = ex.Message.Length > 50 ? ex.Message.Substring(0, 50) : ex.Message;
                _logger.LogError(ex, "SessionValidationMiddleware Error");
            }

            await _next(context);
        }
    }
}
