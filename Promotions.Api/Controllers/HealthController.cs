using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Promotions.Infrastructure.Persistence;
using System.Diagnostics;
using System.Text.Json;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/v1/health")]
    [ApiVersion("1.0")]
    [Tags("Health")]
    public class HealthController : ControllerBase
    {
        private readonly PromotionsDbContext _dbContext;
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public HealthController(PromotionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Basic health check - Quick service status
        /// </summary>
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                service = "Promotions API",
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime()
            });
        }

        /// <summary>
        /// Detailed health check - Database, memory, and dependencies
        /// </summary>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailedHealth()
        {
            var detailedChecks = new
            {
                database = (dynamic)await CheckDatabaseHealth(),
                memory = (dynamic)CheckMemoryHealth(),
                api = (dynamic)CheckApiHealth()
            };

            var healthStatus = new
            {
                service = "Promotions API",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime(),
                checks = detailedChecks
            };

            var isHealthy = detailedChecks.database.status == "Healthy" &&
                           detailedChecks.memory.status == "Healthy" &&
                           detailedChecks.api.status == "Healthy";

            if (!isHealthy)
            {
                // Log alert
                LogServiceAlert(healthStatus);
                return StatusCode(503, healthStatus); // Service Unavailable
            }

            return Ok(healthStatus);
        }

        private async Task<object> CheckDatabaseHealth()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await _dbContext.Database.CanConnectAsync();
                stopwatch.Stop();

                return new
                {
                    status = "Healthy",
                    responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    message = "Database connection successful"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "Unhealthy",
                    responseTime = "N/A",
                    message = "Database connection failed",
                    error = ex.Message
                };
            }
        }

        private object CheckMemoryHealth()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var memoryMB = process.WorkingSet64 / 1024 / 1024;
                var maxMemoryMB = 1024; // 1GB threshold

                var status = memoryMB < maxMemoryMB ? "Healthy" : "Warning";

                return new
                {
                    status,
                    memoryUsage = $"{memoryMB} MB",
                    threshold = $"{maxMemoryMB} MB",
                    message = status == "Healthy" ? "Memory usage normal" : "High memory usage"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "Unknown",
                    memoryUsage = "N/A",
                    message = "Failed to check memory",
                    error = ex.Message
                };
            }
        }

        private object CheckApiHealth()
        {
            return new
            {
                status = "Healthy",
                message = "API is responding",
                endpoints = new
                {
                    promotions = "/api/promotions/actions",
                    products = "/api/promotions/products",
                    participants = "/api/participants/all"
                }
            };
        }

        private string GetUptime()
        {
            var uptime = DateTime.UtcNow - _startTime;
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        private void LogServiceAlert(object healthStatus)
        {
            // This will be logged by Serilog
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("🔴 SERVICE HEALTH ALERT");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine($"Service:     Promotions API");
            Console.WriteLine($"Status:      Unhealthy ❌");
            Console.WriteLine($"Time:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Details:     {JsonSerializer.Serialize(healthStatus, new JsonSerializerOptions { WriteIndented = true })}");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }
    }
}
