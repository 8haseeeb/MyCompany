using Microsoft.AspNetCore.Mvc;
using MyCompany.ApiGateway.Routing;
using System.Diagnostics;
using System.Text.Json;

namespace MyCompany.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/gateway/[controller]")]
    [Tags("Health")]
    public class HealthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IHttpClientFactory httpClientFactory, ILogger<HealthController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemHealth()
        {
            var services = new[]
            {
                new { Name = "Promotions API", Url = $"{RouteResolver.PromotionsBaseUrl}/api/v1/health" },
                new { Name = "SSO API", Url = $"{RouteResolver.SsoBaseUrl}/api/v1/health" }
            };

            var results = new List<object>();
            bool allHealthy = true;

            // Use named client "HealthCheck": TLS validation disabled only in Development (see Program.cs).
            using var client = _httpClientFactory.CreateClient("HealthCheck");

            foreach (var service in services)
            {
                try
                {
                    var stopwatch = Stopwatch.StartNew();
                    var response = await client.GetAsync(service.Url);
                    stopwatch.Stop();

                    var status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
                    if (!response.IsSuccessStatusCode) allHealthy = false;

                    results.Add(new
                    {
                        service = service.Name,
                        url = service.Url,
                        status = status,
                        responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                        statusCode = (int)response.StatusCode
                    });
                }
                catch (Exception ex)
                {
                    allHealthy = false;
                    results.Add(new
                    {
                        service = service.Name,
                        status = "Down",
                        error = ex.Message
                    });

                    LogServiceDownAlert(service.Name, ex.Message);
                }
            }

            var systemStatus = new
            {
                status = allHealthy ? "Healthy" : "Unhealthy",
                timestamp = DateTime.UtcNow,
                services = results
            };

            // We always return Ok(200) so the UI can easily parse the JSON and show which EXACT service is down.
            // If we return 503, some client-side interceptors might swallow the specific error details.
            return Ok(systemStatus);
        }

        private void LogServiceDownAlert(string serviceName, string reason)
        {
            var alert = $@"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔴 GATEWAY ALERT: DETECTED DOWN SERVICE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Service:     {serviceName}
Status:      Down ❌
Time:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Reason:      {reason}
Impact:      Gateway cannot route requests to this service
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";

            _logger.LogError(alert);
            Console.WriteLine(alert);
        }
    }
}
