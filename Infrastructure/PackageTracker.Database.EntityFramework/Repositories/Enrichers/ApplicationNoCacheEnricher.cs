using Microsoft.EntityFrameworkCore;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework.Repositories.Enrichers;
internal class ApplicationNoCacheEnricher(PackageTrackerDbContext dbContext, bool showOnlyTrackedPackages = false) : Common.Enrichers.ApplicationNoCacheEnricher(showOnlyTrackedPackages)
{
    protected override async Task<IDictionary<string, Package>> GetAllPackagesAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Packages.AsNoTracking().Where(p => packagesName.Contains(p.Name)).ToDictionaryAsync(x => x.Name, cancellationToken: cancellationToken);
    }

    protected override async Task<IReadOnlyCollection<Framework>> GetAllFrameworksAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Frameworks.AsNoTracking().ToArrayAsync(cancellationToken);
    }
}
