using Microsoft.AspNetCore.Http;

namespace MyCompany.ApiGateway.Routing
{
    public static class RouteResolver
    {
        public static string Resolve(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            if (path.StartsWith("/api/promotions"))
                return "https://localhost:7043";

            if (path.StartsWith("/api/auth"))
                return "https://localhost:7222";

            return "https://localhost:7043";
        }

    }
}
