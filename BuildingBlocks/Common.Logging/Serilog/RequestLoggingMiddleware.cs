using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MyCompany.Common.Logging.Serilog
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public RequestLoggingMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? "N/A";

            string requestBody = "";
            string responseBodyText = "";
            Stream? originalBodyStream = null;
            MemoryStream? responseBody = null;

            if (_env.IsDevelopment())
            {
                // --- Development: capture request body ---
                if (context.Request.ContentLength > 0 && context.Request.Body.CanSeek)
                {
                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                // --- Development: capture response body ---
                originalBodyStream = context.Response.Body;
                responseBody = new MemoryStream();
                context.Response.Body = responseBody;
            }

            await _next(context);

            stopwatch.Stop();

            if (_env.IsDevelopment() && responseBody != null && originalBodyStream != null)
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
                responseBody.Dispose();
            }

            var userName = context.User?.Identity?.Name ?? "Anonymous";
            var userId = context.User?.FindFirst("sub")?.Value ?? "N/A";
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (_env.IsDevelopment())
            {
                Log.Information(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms | User: {UserName} ({UserId}) | IP: {ClientIp} | CorrelationId: {CorrelationId} | RequestBody: {RequestBody} | ResponseBody: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    userName,
                    userId,
                    clientIp,
                    correlationId,
                    requestBody,
                    responseBodyText);
            }
            else
            {
                // Production: no request/response body (avoids PII and secrets in logs).
                Log.Information(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms | User: {UserName} ({UserId}) | IP: {ClientIp} | CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds,
                    userName,
                    userId,
                    clientIp,
                    correlationId);
            }
        }
    }
}
