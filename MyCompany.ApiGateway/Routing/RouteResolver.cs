using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MyCompany.ApiGateway.Routing
{
    public static class RouteResolver
    {
        private static string _promotionsUrl = "http://localhost:5137";
        private static string _ssoUrl = "http://localhost:5253";

        /// <summary>
        /// Call once at startup (after configuration is loaded). Environment variables win over appsettings Downstream section.
        /// </summary>
        public static void Configure(IConfiguration configuration)
        {
            var section = configuration.GetSection("Downstream");
            _promotionsUrl = (Environment.GetEnvironmentVariable("PROMOTIONS_URL")
                ?? section["PromotionsUrl"]
                ?? _promotionsUrl).TrimEnd('/');
            _ssoUrl = (Environment.GetEnvironmentVariable("SSO_URL")
                ?? section["SsoUrl"]
                ?? _ssoUrl).TrimEnd('/');
        }

        public static string PromotionsBaseUrl => _promotionsUrl;
        public static string SsoBaseUrl => _ssoUrl;

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
                return _promotionsUrl;
            return _promotionsUrl;
        }

        /// <summary>Rewrite /api/... to /api/v1/... for backward compatibility when no version segment present.</summary>
        public static string RewriteToVersionedPath(string? path)
        {
            if (string.IsNullOrEmpty(path)) return path ?? "";
            if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase)) return path;
            // Do not rewrite gateway-owned paths (e.g. /api/gateway/health)
            if (path.StartsWith("/api/gateway/", StringComparison.OrdinalIgnoreCase)) return path;
            // Gateway liveness at /api/health (handled by controller, not proxied)
            if (string.Equals(path, "/api/health", StringComparison.OrdinalIgnoreCase)) return path;
            // Already versioned: /api/v1/... or /api/v2/...
            if (path.Length > 6 && path[5] == 'v' && char.IsDigit(path[6]))
                return path;
            return "/api/v1" + path.Substring(4);
        }
    }
}
