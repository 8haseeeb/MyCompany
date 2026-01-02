using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MyCompany.Common.Logging
{
    public static class SerilogExtensions
    {
        public static IHostBuilder AddSerilogLogging(
            this IHostBuilder host,
            IConfiguration configuration,
            string applicationName)
        {
            return host.UseSerilog((context, services, loggerConfig) =>
            {
                loggerConfig
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
    }
}
