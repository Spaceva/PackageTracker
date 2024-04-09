using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MongoDb.Repositories;
internal class FrameworksDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IFrameworkRepository? cacheRepository, MongoDbContext dbContext, ILogger<FrameworkDbModel> logger) : BaseDbRepository<FrameworkDbModel>(dbContext, logger), IFrameworkRepository
{
    public async Task<bool> ExistsAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        if (cacheRepository is null)
        {
            return await ExistsNoCacheAsync(name, version, cancellationToken);
        }

        return (await cacheRepository.ExistsAsync(name, version, cancellationToken)) || (await ExistsNoCacheAsync(name, version, cancellationToken));
    }

    public async Task DeleteByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        await DeleteByQueryAsync(Filter.Eq(f => f.Name, name) & Filter.Eq(f => f.Version, version), cancellationToken);
        if (cacheRepository is not null)
        {
            await cacheRepository.DeleteByVersionAsync(name, version, cancellationToken);
        }
    }

    public async Task<Framework> GetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        return await TryGetByVersionAsync(name, version, cancellationToken) ?? throw new FrameworkNotFoundException();
    }

    public async Task SaveAsync(Framework framework, CancellationToken cancellationToken = default)
    {
        var frameworkDb = new FrameworkDbModel(framework);
        await UpdateAsync(Filter.Eq(f => f.Name, framework.Name) & Filter.Eq(f => f.Version, framework.Version), frameworkDb, cancellationToken);
        if (cacheRepository is not null)
        {
            await cacheRepository.SaveAsync(framework, cancellationToken);
        }
    }

    public async Task<IReadOnlyCollection<Framework>> SearchAsync(FrameworkSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        return [.. await FindAsync(SearchCriteria(searchCriteria), cancellationToken)];
    }

    public async Task<Framework?> TryGetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        if (cacheRepository is null)
        {
            return await TryGetByVersionNoCacheAsync(name, version, cancellationToken);
        }

        var framework = await cacheRepository.TryGetByVersionAsync(name, version, cancellationToken);
        if (framework is not null)
        {
            return framework;
        }

        framework = await TryGetByVersionNoCacheAsync(name, version, cancellationToken);
        if (framework is not null)
        {
            await cacheRepository.SaveAsync(framework, cancellationToken);
        }

        return framework;
    }

    private static FilterDefinition<FrameworkDbModel> SearchCriteria(FrameworkSearchCriteria? searchCriteria)
    {
        var filter = Filter.Empty;
        if (searchCriteria is null)
        {
            return filter;
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.Name))
        {
            filter &= Filter.Regex(f => f.Name, new MongoDB.Bson.BsonRegularExpression("/" + searchCriteria.Name + "/"));
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.Version))
        {
            filter &= Filter.Eq(f => f.Version, searchCriteria.Version);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.CodeName))
        {
            filter &= Filter.Eq(f => f.CodeName, searchCriteria.CodeName);
        }

        if (searchCriteria.Channel is not null && searchCriteria.Channel.Count > 0)
        {
            filter &= Filter.In(nameof(FrameworkDbModel.Channel), searchCriteria.Channel);
        }

        if (searchCriteria.Status is not null && searchCriteria.Status.Count > 0)
        {
            filter &= Filter.In(nameof(FrameworkDbModel.Status), searchCriteria.Status);
        }

        if (searchCriteria.ReleaseDateMinimum.HasValue)
        {
            filter &= Filter.Gte(f => f.ReleaseDate, searchCriteria.ReleaseDateMinimum);
        }

        if (searchCriteria.ReleaseDateMaximum.HasValue)
        {
            filter &= Filter.Lte(f => f.ReleaseDate, searchCriteria.ReleaseDateMaximum);
        }

        if (searchCriteria.EndOfLifeMinimum.HasValue)
        {
            filter &= Filter.Gte(f => f.EndOfLife, searchCriteria.EndOfLifeMinimum);
        }

        if (searchCriteria.EndOfLifeMaximum.HasValue)
        {
            filter &= Filter.Lte(f => f.EndOfLife, searchCriteria.EndOfLifeMaximum);
        }

        return filter;
    }

    private async Task<Framework?> TryGetByVersionNoCacheAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var frameworks = await FindAsync(Filter.Eq(f => f.Name, name) & Filter.Eq(f => f.Version, version), cancellationToken);
        return frameworks.SingleOrDefault();
    }

    private async Task<bool> ExistsNoCacheAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        return await AnyAsync(Filter.Eq(f => f.Name, name) & Filter.Eq(f => f.Version, version), cancellationToken);
    }
}
