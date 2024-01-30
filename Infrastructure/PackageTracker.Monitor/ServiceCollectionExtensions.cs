using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Monitor;

public static class ServiceCollectionExtensions
{
    public static IMonitorRegistrator AddMonitor(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScopedBackgroundService<MonitorBackgroundService>();

        services.Configure<MonitorSettings>(configuration.GetSection("Monitor"));

        return new MonitorRegistrator(services);
    }

    public static IConfigurationBuilder AddMonitorConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("monitor.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"monitor.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
