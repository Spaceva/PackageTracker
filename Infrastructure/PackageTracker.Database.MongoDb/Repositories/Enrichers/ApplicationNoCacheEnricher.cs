using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MongoDb.Repositories.Enrichers;
internal class ApplicationNoCacheEnricher(MongoDbContext dbContext, bool showOnlyTrackedPackages = false) : Common.Enrichers.ApplicationNoCacheEnricher(showOnlyTrackedPackages)
{
    protected override async Task<IDictionary<string, Package>> GetAllPackagesAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken = default)
    {
        var collection = dbContext.GetCollection<PackageDbModel>();
        var packagesCursor = await collection.FindAsync(Builders<PackageDbModel>.Filter.In(p => p.Name, packagesName), cancellationToken: cancellationToken);
        var packages = await packagesCursor.ToListAsync(cancellationToken: cancellationToken);
        return packages.Select(p => p.ToDomain()).ToDictionary(p => p.Name);
    }

    protected override async Task<IReadOnlyCollection<Framework>> GetAllFrameworksAsync(CancellationToken cancellationToken)
    {
        var collection = dbContext.GetCollection<FrameworkDbModel>();
        var frameworksCursor = await collection.FindAsync(Builders<FrameworkDbModel>.Filter.Empty, cancellationToken: cancellationToken);
        return await frameworksCursor.ToListAsync(cancellationToken: cancellationToken);
    }
}