using MyCompany.ApiGateway.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MyCompany.ApiGateway.UnitTests.Middlewares
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ExceptionHandlingMiddleware _middleware;

        public ExceptionHandlingMiddlewareTests()
        {
            _next = Substitute.For<RequestDelegate>();
            _logger = Substitute.For<ILogger<ExceptionHandlingMiddleware>>();
            _middleware = new ExceptionHandlingMiddleware(_next, _logger);
        }

        [Fact]
        public async Task InvokeAsync_Should_HandleException_And_SetStatusCode()
        {
            // --- ARRANGE ---
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _next(Arg.Any<HttpContext>()).Returns(_ => throw new KeyNotFoundException("Not found"));

            // --- ACT ---
            await _middleware.InvokeAsync(context);

            // --- ASSERT ---
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);
            Assert.Equal("http://localhost:5173", context.Response.Headers["Access-Control-Allow-Origin"]);
        }

        [Fact]
        public async Task InvokeAsync_Should_HandleGenericException_AsInternalServerError()
        {
            // --- ARRANGE ---
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _next(Arg.Any<HttpContext>()).Returns(_ => throw new Exception("Unexpected error"));

            // --- ACT ---
            await _middleware.InvokeAsync(context);

            // --- ASSERT ---
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        }
    }
}
