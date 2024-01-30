using Microsoft.Extensions.DependencyInjection;

namespace PackageTracker.Infrastructure.BackgroundServices;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedBackgroundService<TScopedBackgroundService>(this IServiceCollection services)
        where TScopedBackgroundService : class, IScopedBackgroundService
    {
        services.AddHostedService<ScopedHostedService<TScopedBackgroundService>>();

        services.AddScoped<TScopedBackgroundService>();

        services.AddScoped<IScopedBackgroundService>(x => x.GetRequiredService<TScopedBackgroundService>());

        return services;
    }
}
