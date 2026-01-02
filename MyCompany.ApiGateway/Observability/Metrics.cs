using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyCompany.ApiGateway.Observability
{
    public class Metrics
    {
        public static async Task Record(HttpContext context, RequestDelegate next)
        {
            var start = DateTime.UtcNow;
            await next(context);
            var duration = DateTime.UtcNow - start;
            Console.WriteLine($"Request to {context.Request.Path} took {duration.TotalMilliseconds}ms");
        }
    }
}
