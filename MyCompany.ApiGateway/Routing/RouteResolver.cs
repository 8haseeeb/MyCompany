using Microsoft.AspNetCore.Http;

namespace MyCompany.ApiGateway.Routing
{
    public static class RouteResolver
    {
        public static string Resolve(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path.StartsWith("/api/promotions"))
                return "http://localhost:5137";

            if (path.StartsWith("/api/auth"))
                return "http://localhost:5253";

            return "http://localhost:5137";
        }

    }
}
