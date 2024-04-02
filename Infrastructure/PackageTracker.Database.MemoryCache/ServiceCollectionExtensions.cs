using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.MemoryCache.BackgroundServices;
using PackageTracker.Database.MemoryCache.Repositories;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Database.MemoryCache;
public static class ServiceCollectionExtensions
{
    public static void WithMemoryCache(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddInMemoryRepositories();
        services.AddScopedBackgroundService<CacheInitializerBackgroundService>();
    }

    private static void AddInMemoryRepositories(this IServiceCollection services)
    {
        services.AddKeyedSingleton<IPackagesRepository, PackagesInMemoryRepository>(Constants.SERVICEKEY);
        services.AddKeyedSingleton<IFrameworkRepository, FrameworkInMemoryRepository>(Constants.SERVICEKEY);
    }
}
