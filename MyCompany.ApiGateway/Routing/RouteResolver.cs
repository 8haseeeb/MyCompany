using Microsoft.AspNetCore.Http;

namespace MyCompany.ApiGateway.Routing
{
    public static class RouteResolver
    {
        private static readonly string _promotionsUrl = Environment.GetEnvironmentVariable("PROMOTIONS_URL") ?? "http://localhost:5137";
        private static readonly string _ssoUrl = Environment.GetEnvironmentVariable("SSO_URL") ?? "http://localhost:5253";

        public static string Resolve(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path.StartsWith("/api/promotions"))
                return _promotionsUrl;

            if (path.StartsWith("/api/auth"))
                return _ssoUrl;

            return _promotionsUrl;
        }

    }
}
