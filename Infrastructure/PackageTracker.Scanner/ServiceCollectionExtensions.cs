using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Scanner;

public static class ServiceCollectionExtensions
{
    public static IScannerRegistrator AddScanner(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScopedBackgroundService<ScannerBackgroundService>();

        services.Configure<ScannerSettings>(configuration.GetSection("Scanner"));

        return new ScannerRegistrator(services);
    }

    public static IConfigurationBuilder AddScannerConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("scanner.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"scanner.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
