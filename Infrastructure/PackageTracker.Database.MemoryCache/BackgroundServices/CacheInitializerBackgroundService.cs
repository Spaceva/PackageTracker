using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Database.MemoryCache.BackgroundServices;
internal class CacheInitializerBackgroundService(
    [FromKeyedServices(Constants.SERVICEKEY)] IPackagesRepository cachePackagesRepository,
    [FromKeyedServices(Constants.SERVICEKEY)] IFrameworkRepository cacheFrameworksRepository,
    IPackagesRepository dbPackagesRepository,
    IFrameworkRepository dbFrameworkRepository,
    ILogger<CacheInitializerBackgroundService> logger) : IScopedBackgroundService
{
    public async Task RunAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initializing cache...");
        await LoadFrameworksAsync(stoppingToken);
        await LoadPackagesAsync(stoppingToken);
        logger.LogInformation("Cache initialized.");
    }

    private async Task LoadFrameworksAsync(CancellationToken stoppingToken)
    {
        var allFrameworks = await dbFrameworkRepository.SearchAsync(new FrameworkSearchCriteria(), cancellationToken: stoppingToken);
        foreach (var framework in allFrameworks)
        {
            await cacheFrameworksRepository.SaveAsync(framework, stoppingToken);
        }
        logger.LogInformation("{AllFrameworksCount} {ObjectName}(s) loaded.", allFrameworks.Count, nameof(Framework));
    }

    private async Task LoadPackagesAsync(CancellationToken stoppingToken)
    {
        var allPackages = await dbPackagesRepository.GetAllAsync(cancellationToken: stoppingToken);
        foreach (var package in allPackages)
        {
            await cachePackagesRepository.AddAsync(package, stoppingToken);
        }
        logger.LogInformation("{AllPackagesCount} {ObjectName}(s) loaded.", allPackages.Count, nameof(Package));
    }
}
