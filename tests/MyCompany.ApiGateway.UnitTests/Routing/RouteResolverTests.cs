using MyCompany.ApiGateway.Routing;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MyCompany.ApiGateway.UnitTests.Routing
{
    public class RouteResolverTests
    {
        [Theory]
        [InlineData("/api/promotions", "http://localhost:5137")]
        [InlineData("/API/PROMOTIONS/something", "http://localhost:5137")]
        [InlineData("/api/auth/login", "http://localhost:5253")]
        [InlineData("/other", "http://localhost:5137")]
        public void Resolve_Should_ReturnCorrectBaseUrl(string path, string expectedBaseUrl)
        {
            // --- ARRANGE ---
            var context = new DefaultHttpContext();
            context.Request.Path = path;

            // --- ACT ---
            var actualBaseUrl = RouteResolver.Resolve(context);

            // --- ASSERT ---
            Assert.Equal(expectedBaseUrl, actualBaseUrl);
        }

        [Theory]
        [InlineData("/api/v1/promotions/dashboard", "http://localhost:5137")]
        [InlineData("/api/v1/auth/login", "http://localhost:5253")]
        [InlineData("/api/v1/actions/1/participants", "http://localhost:5137")]
        public void ResolveByPath_Should_ReturnCorrectBaseUrl_ForVersionedPaths(string path, string expectedBaseUrl)
        {
            var actualBaseUrl = RouteResolver.ResolveByPath(path);
            Assert.Equal(expectedBaseUrl, actualBaseUrl);
        }

        [Theory]
        [InlineData("/api/promotions/dashboard", "/api/v1/promotions/dashboard")]
        [InlineData("/api/auth/login", "/api/v1/auth/login")]
        [InlineData("/api/v1/promotions/dashboard", "/api/v1/promotions/dashboard")]
        [InlineData("/api/v2/auth/login", "/api/v2/auth/login")]
        [InlineData("/api/gateway/health", "/api/gateway/health")]
        [InlineData("/api/health", "/api/health")]
        [InlineData("/other", "/other")]
        [InlineData("", "")]
        public void RewriteToVersionedPath_Should_AddV1_WhenNoVersion(string path, string expected)
        {
            var actual = RouteResolver.RewriteToVersionedPath(path);
            Assert.Equal(expected, actual);
        }
    }
}
