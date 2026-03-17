using Microsoft.AspNetCore.Http;

namespace MyCompany.ApiGateway.Routing
{
    public static class RouteResolver
    {
        private static readonly string _promotionsUrl = Environment.GetEnvironmentVariable("PROMOTIONS_URL") ?? "http://localhost:5137";
        private static readonly string _ssoUrl = Environment.GetEnvironmentVariable("SSO_URL") ?? "http://localhost:5253";

        /// <summary>Resolve base URL from request path (supports both /api/... and /api/v1/...).</summary>
        public static string Resolve(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            return ResolveByPath(path);
        }

        /// <summary>Resolve base URL from path string (e.g. after version rewrite).</summary>
        public static string ResolveByPath(string path)
        {
            if (path.StartsWith("/api/promotions") || path.StartsWith("/api/v1/promotions"))
                return _promotionsUrl;
            if (path.StartsWith("/api/auth") || path.StartsWith("/api/v1/auth"))
                return _ssoUrl;
            if (path.StartsWith("/api/actions") || path.StartsWith("/api/v1/actions"))
                return _promotionsUrl;
            if (path.StartsWith("/api/health") || path.StartsWith("/api/v1/health"))
                return _promotionsUrl; // or SSO when path is under auth backend
            return _promotionsUrl;
        }

        /// <summary>Rewrite /api/... to /api/v1/... for backward compatibility when no version segment present.</summary>
        public static string RewriteToVersionedPath(string? path)
        {
            if (string.IsNullOrEmpty(path)) return path ?? "";
            if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)) return path;
            // Do not rewrite gateway-owned paths (e.g. /api/gateway/health)
            if (path.StartsWith("/api/gateway/", StringComparison.OrdinalIgnoreCase)) return path;
            // Already versioned: /api/v1/... or /api/v2/...
            if (path.Length > 6 && path[5] == 'v' && char.IsDigit(path[6]))
                return path;
            return "/api/v1" + path.Substring(4);
        }
    }
}
