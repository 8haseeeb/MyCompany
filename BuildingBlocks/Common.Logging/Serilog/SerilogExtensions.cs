using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MyCompany.Common.Logging
{
    public static class SerilogExtensions
    {
        private static LoggingLevelSwitch _levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

        public static IHostBuilder AddSerilogLogging(
            this IHostBuilder host,
            IConfiguration configuration,
            string applicationName)
        {
            return host.UseSerilog((context, services, loggerConfig) =>
            {
                loggerConfig
                    .MinimumLevel.ControlledBy(_levelSwitch)
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", applicationName)
                    .Enrich.WithMachineName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .WriteTo.Console()
                    .WriteTo.File(
                        $"logs/{applicationName}-.log",
                        rollingInterval: RollingInterval.Day);
            });
        }

        public static IServiceCollection AddLoggingLevelSwitch(this IServiceCollection services)
        {
            services.AddSingleton(_levelSwitch);
            return services;
        }
    }
}
