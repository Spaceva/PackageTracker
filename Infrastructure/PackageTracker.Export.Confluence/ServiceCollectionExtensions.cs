using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackageTracker.Export.Confluence.Core;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Export.Confluence;

public static class ServiceCollectionExtensions
{
    public static IConfluenceExportRegistrator AddConfluenceExport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConfluenceSettings>(configuration.GetSection("Confluence"));
        
        services.AddScoped<ConfluenceClientWrapper>();
        
        services.AddScopedBackgroundService<ConfluenceExportBackgroundService>();
        
        return new ConfluenceExportRegistrator(services);
    }

    public static IConfigurationBuilder AddConfluenceConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("confluence.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"confluence.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
