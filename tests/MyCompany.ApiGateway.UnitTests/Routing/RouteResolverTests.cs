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
    }
}
