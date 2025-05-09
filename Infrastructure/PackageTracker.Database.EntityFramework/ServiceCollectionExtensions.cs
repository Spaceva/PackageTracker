﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.EntityFramework.Repositories;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Modules;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Package;

namespace PackageTracker.Database.EntityFramework;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerEFDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbRepositories();
        services.AddSqlServer<PackageTrackerDbContext>(configuration.GetSection("Persistence:ConnectionString").Value!);
        return services;
    }
    
    public static IServiceCollection AddInMemoryEFDatabase(this IServiceCollection services)
    {
        services.AddDbRepositories();
        services.AddDbContext<PackageTrackerDbContext>(opt => opt.UseInMemoryDatabase(nameof(PackageTrackerDbContext)));
        return services;
    }

    public static IServiceCollection AddPostgresEFDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbRepositories();
        services.AddNpgsql<PackageTrackerDbContext>(configuration.GetSection("Persistence:ConnectionString").Value!);
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
        services.AddScoped<IFrameworkRepository, FrameworksDbRepository>();
        services.AddSingleton<IModuleManager, ModulesDbRepository>();
    }
}
