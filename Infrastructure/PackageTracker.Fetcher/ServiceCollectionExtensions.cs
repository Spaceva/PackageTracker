using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Fetcher;

public static class ServiceCollectionExtensions
{
    public static IFetcherRegistrator AddFetcher(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScopedBackgroundService<FetcherBackgroundService>();

        services.Configure<FetcherSettings>(configuration.GetSection("Fetcher"));

        return new FetcherRegistrator(services);
    }

    public static IConfigurationBuilder AddFetcherConfiguration(this IConfigurationBuilder configuration, IHostEnvironment environment)
     => configuration.AddJsonFile("fetcher.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($"fetcher.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}
