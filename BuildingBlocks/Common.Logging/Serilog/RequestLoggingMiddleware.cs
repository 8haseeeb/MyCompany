using Microsoft.AspNetCore.Http;
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

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // --- Capture Request Body ---
            string requestBody = "";
            if (context.Request.ContentLength > 0 &&
                context.Request.Body.CanSeek)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(
                    context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // --- Capture Response Body ---
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            stopwatch.Stop();

            // --- Extract User Info from JWT if available ---
            var userName = context.User?.Identity?.Name ?? "Anonymous";
            var userId = context.User?.FindFirst("sub")?.Value ?? "N/A";
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // --- Log structured information ---
            Log.Information("HTTP {Method} {Path} responded {StatusCode} in {Elapsed:0.0000} ms | User: {UserName} ({UserId}) | IP: {ClientIp} | RequestBody: {RequestBody} | ResponseBody: {ResponseBody}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds,
                userName,
                userId,
                clientIp,
                requestBody,
                responseBodyText
            );

            // --- Copy response back to original stream ---
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
