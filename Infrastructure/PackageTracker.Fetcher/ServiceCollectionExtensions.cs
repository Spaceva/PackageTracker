using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Fetcher;

public static class ServiceCollectionExtensions
{
    public static void ConfigureFetcher(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<FetcherBackgroundService>();

        services.Configure<FetcherSettings>(configuration.GetSection("Fetcher"));
    }
}
