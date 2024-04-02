using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Package;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEFDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<PackageTrackerDbContext>(configuration.GetConnectionString("Database"));
        services.AddDbRepositories();
        return services;
    }

    public static async Task EnsureDatabaseIsUpdatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    private static void AddDbRepositories(this IServiceCollection services)
    {
        services.AddScoped<INotificationsRepository, NotificationsDbRepository>();
        services.AddScoped<IApplicationsRepository, ApplicationsDbRepository>();
        services.AddScoped<IPackagesRepository, PackagesDbRepository>();
        services.AddScoped<IFrameworkRepository, FrameworkDbRepository>();
    }
}
