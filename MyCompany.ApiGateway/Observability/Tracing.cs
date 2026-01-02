using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MyCompany.ApiGateway.Observability
{
    public class Tracing
    {
        public static async Task Trace(HttpContext context, RequestDelegate next)
        {
            Console.WriteLine($"Trace - {context.Request.Method} {context.Request.Path}");
            await next(context);
        }
    }
}
