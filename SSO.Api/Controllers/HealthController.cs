using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSO.Infrastructure.Persistence;
using System.Diagnostics;
using System.Text.Json;

namespace SSO.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Health")]
    public class HealthController : ControllerBase
    {
        private readonly IdentityDbContext _dbContext; // Changed from ApplicationDbContext
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public HealthController(IdentityDbContext dbContext) // Changed from ApplicationDbContext
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                service = "SSO API",
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime()
            });
        }

        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailedHealth()
        {
            var detailedChecks = new
            {
                database = (dynamic)await CheckDatabaseHealth(),
                memory = (dynamic)CheckMemoryHealth()
            };

            var healthStatus = new
            {
                service = "SSO API",
                version = "1.0.0",
                timestamp = DateTime.UtcNow,
                uptime = GetUptime(),
                checks = detailedChecks
            };

            var isHealthy = detailedChecks.database.status == "Healthy" &&
                           detailedChecks.memory.status == "Healthy";

            if (!isHealthy)
            {
                return StatusCode(503, healthStatus);
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
                return new
                {
                    status = "Healthy",
                    memoryUsage = $"{memoryMB} MB"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    status = "Unknown",
                    error = ex.Message
                };
            }
        }

        private string GetUptime()
        {
            var uptime = DateTime.UtcNow - _startTime;
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }
    }
}
