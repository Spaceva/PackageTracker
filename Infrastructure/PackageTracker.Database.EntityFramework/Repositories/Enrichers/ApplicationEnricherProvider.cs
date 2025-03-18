using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.Common.Enrichers;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package;

namespace PackageTracker.Database.EntityFramework.Repositories.Enrichers;
internal static class ApplicationEnricherProvider
{
    public static IApplicationEnricher GetApplicationEnricher(this IServiceProvider serviceProvider, PackageTrackerDbContext dbContext, bool showOnlyTrackedPackages = false)
    {
        var frameworksRepository = serviceProvider.GetKeyedService<IFrameworkRepository>(MemoryCache.Constants.SERVICEKEY);
        var packagesRepository = serviceProvider.GetKeyedService<IPackagesRepository>(MemoryCache.Constants.SERVICEKEY);

        var hasCache = frameworksRepository is not null && packagesRepository is not null;
        if (hasCache)
        {
            return new ApplicationWithCacheEnricher(packagesRepository!, frameworksRepository!, showOnlyTrackedPackages);
        }

        return new ApplicationNoCacheEnricher(dbContext);
    }
}
