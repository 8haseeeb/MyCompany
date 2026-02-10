using Promotions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Promotions.Api.Services
{
    public class HealthMonitoringService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HealthMonitoringService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public HealthMonitoringService(
            IServiceProvider serviceProvider,
            ILogger<HealthMonitoringService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(" Health Monitoring Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckServiceHealth();
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in health monitoring service");
                }
            }
        }

        private async Task CheckServiceHealth()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PromotionsDbContext>();

            try
            {
                // Check database connectivity
                var canConnect = await dbContext.Database.CanConnectAsync();

                if (!canConnect)
                {
                    LogServiceDownAlert("Database", "Cannot connect to database");
                }
            }
            catch (Exception ex)
            {
                LogServiceDownAlert("Database", ex.Message);
            }
        }

        private void LogServiceDownAlert(string componentName, string reason)
        {
            var alert = $@"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
🔴 SERVICE DOWN ALERT
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Service:     Promotions API
Component:   {componentName}
Status:      Unhealthy ❌
Time:        {DateTime.Now:yyyy-MM-dd HH:mm:ss}
Reason:      {reason}
Impact:      Service may not be functioning properly
Action:      Check {componentName.ToLower()} connectivity and logs
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";

            _logger.LogError(alert);
            Console.WriteLine(alert);
        }
    }
}
