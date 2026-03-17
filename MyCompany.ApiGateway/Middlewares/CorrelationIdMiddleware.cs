using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyCompany.ApiGateway.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                ?? System.Guid.NewGuid().ToString();
            context.Request.Headers["X-Correlation-ID"] = correlationId;

            await _next(context);

            // Echo back so clients and downstream can correlate requests with logs.
            context.Response.Headers["X-Correlation-ID"] = correlationId;
        }
    }
}
