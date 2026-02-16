using MyCompany.ApiGateway.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyCompany.ApiGateway.UnitTests.Middlewares
{
    public class CorrelationIdMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_Should_AddCorrelationId_IfMissing()
        {
            // --- ARRANGE ---
            var context = new DefaultHttpContext();
            RequestDelegate next = (innerContext) => Task.CompletedTask;
            var middleware = new CorrelationIdMiddleware(next);

            // --- ACT ---
            await middleware.InvokeAsync(context);

            // --- ASSERT ---
            Assert.True(context.Request.Headers.ContainsKey("X-Correlation-ID"));
            Assert.False(string.IsNullOrWhiteSpace(context.Request.Headers["X-Correlation-ID"]));
        }

        [Fact]
        public async Task InvokeAsync_Should_NotOverWriteCorrelationId_IfPresent()
        {
            // --- ARRANGE ---
            var context = new DefaultHttpContext();
            var existingId = "existing-id";
            context.Request.Headers["X-Correlation-ID"] = existingId;
            
            RequestDelegate next = (innerContext) => Task.CompletedTask;
            var middleware = new CorrelationIdMiddleware(next);

            // --- ACT ---
            await middleware.InvokeAsync(context);

            // --- ASSERT ---
            Assert.Equal(existingId, context.Request.Headers["X-Correlation-ID"]);
        }
    }
}
