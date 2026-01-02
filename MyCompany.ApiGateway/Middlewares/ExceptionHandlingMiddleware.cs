using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyCompany.ApiGateway.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                //  Log full exception with stack trace
                _logger.LogError(ex, "Unhandled exception occurred while processing request");

                context.Response.ContentType = "application/json";

                //  Map exception types to HTTP status codes
                var statusCode = ex switch
                {
                    ArgumentNullException => HttpStatusCode.BadRequest,
                    ArgumentException => HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    NotImplementedException => HttpStatusCode.NotImplemented,
                    TimeoutException => HttpStatusCode.RequestTimeout,
                    _ => HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = (int)statusCode;

                // Standard API error response
                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "An unexpected error occurred. Please contact support.",
                    exceptionType = ex.GetType().Name,

             #if DEBUG
                    // Stack exception details (ONLY in development)
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
             #endif
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response)
                );
            }
        }
    }
}
