using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackageTracker.Domain.Application;
using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Scanner;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationsScanner(this IServiceCollection services, IConfiguration configuration)
    {
        var scannerConfigurationSection = configuration.GetRequiredSection("Scanner");

        var scannerSettings = scannerConfigurationSection.Get<ScannerSettings>();

        ArgumentNullException.ThrowIfNull(scannerSettings);

        services.AddScopedBackgroundService<ScannerBackgroundService>();

        services.Configure<ScannerSettings>(scannerConfigurationSection);

        foreach (var trackedApplication in scannerSettings.Applications)
        {
            services.AddScoped(sp =>
            {
                var mediator = sp.GetRequiredService<IMediator>();
                var parsers = sp.GetServices<IApplicationModuleParser>();
                var registrator = sp.GetRequiredKeyedService<IScannerRegistrator>(trackedApplication.ScannerType);
                return registrator.Register(sp, scannerSettings, trackedApplication, parsers, mediator);
            });
        }

        return services;
    }

    public static IConfigurationBuilder AddApplicationScannerConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("scanner.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"scanner.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
