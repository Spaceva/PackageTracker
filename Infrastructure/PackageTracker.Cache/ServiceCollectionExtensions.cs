using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Packages;
using PackageTracker.Infrastructure.Repositories;

namespace PackageTracker.Cache;

public static class ServiceCollectionExtensions
{
    public static void ConfigureFileCaches(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new PackagesFileContext(configuration["DbFiles:Packages"]))
                .AddSingleton<PackagesCacheRepository>()
                .AddSingleton<ICacheRepository>(x => x.GetRequiredService<PackagesCacheRepository>())
                .AddSingleton<IPackagesRepository>(x => x.GetRequiredService<PackagesCacheRepository>())
                .AddSingleton<ICache>(x => new Cache(x.GetRequiredService<PackagesCacheRepository>(), x.GetRequiredService<PackagesFileContext>(), x.GetRequiredService<ILogger<Cache>>()));

        services.AddSingleton(new NotificationsFileContext(configuration["DbFiles:Notifications"]))
                .AddSingleton<NotificationsCacheRepository>()
                .AddSingleton<ICacheRepository>(x => x.GetRequiredService<NotificationsCacheRepository>())
                .AddSingleton<INotificationsRepository>(x => x.GetRequiredService<NotificationsCacheRepository>())
                .AddSingleton<ICache>(x => new Cache(x.GetRequiredService<NotificationsCacheRepository>(), x.GetRequiredService<NotificationsFileContext>(), x.GetRequiredService<ILogger<Cache>>()));

        services.AddHostedService<CacheBackgroundService>()
                .Configure<CacheSettings>(configuration.GetSection("Caches"));
    }
}
