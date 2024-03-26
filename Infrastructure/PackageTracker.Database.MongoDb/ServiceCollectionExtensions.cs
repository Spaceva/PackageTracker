using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Repositories;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Package;

namespace PackageTracker.Database.MongoDb;
public static class ServiceCollectionExtensions
{
    public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new MongoDbContext(configuration.GetConnectionString("Database")!));
        services.AddDbRepositories();
    }

    private static void AddDbRepositories(this IServiceCollection services)
    {
        services.AddScoped<INotificationsRepository, NotificationsDbRepository>();
        services.AddScoped<IApplicationsRepository, ApplicationsDbRepository>();
        services.AddScoped<IPackagesRepository, PackagesDbRepository>();
        services.AddScoped<IFrameworkRepository, FrameworksDbRepository>();
    }
}
