using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.EntityFramework.Extensions;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.EntityFramework;
internal class FrameworksDbRepository(IServiceScopeFactory serviceScopeFactory) : IFrameworkRepository
{
    public async Task<bool> ExistsAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cacheRepository = scope.ServiceProvider.GetKeyedService<IFrameworkRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is null)
        {
            return await ExistsNoCacheAsync(name, version, cancellationToken);
        }

        return (await cacheRepository.ExistsAsync(name, version, cancellationToken)) || (await ExistsNoCacheAsync(name, version, cancellationToken));
    }

    public async Task<Framework> GetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
        => await TryGetByVersionAsync(name, version, cancellationToken) ?? throw new FrameworkNotFoundException();

    public async Task<Framework?> TryGetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cacheRepository = scope.ServiceProvider.GetKeyedService<IFrameworkRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is null)
        {
            return await TryGetByVersionNoCacheAsync(name, version, cancellationToken);
        }

        var cachedFramework = await cacheRepository.TryGetByVersionAsync(name, version, cancellationToken);
        if (cachedFramework is not null)
        {
            return cachedFramework;
        }

        var framework = await TryGetByVersionNoCacheAsync(name, version, cancellationToken);
        if (framework is not null)
        {
            await cacheRepository.SaveAsync(framework, cancellationToken);
        }

        return framework;
    }

    public async Task DeleteByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var existingFramework = await dbContext.Frameworks.FindAsync([name, version], cancellationToken) ?? throw new FrameworkNotFoundException();

        dbContext.Frameworks.Remove(existingFramework);
        dbContext.SaveChanges();

        var cacheRepository = scope.ServiceProvider.GetKeyedService<IFrameworkRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is not null)
        {
            await cacheRepository.DeleteByVersionAsync(name, version, cancellationToken);
        }
    }

    public async Task SaveAsync(Framework framework, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var existingFramework = await dbContext.Frameworks.FindAsync([framework.Name, framework.Version], cancellationToken);
        if (existingFramework is null)
        {
            dbContext.Frameworks.Add(framework);
        }
        else
        {
            existingFramework.EndOfLife = framework.EndOfLife;
            existingFramework.Channel = framework.Channel;
            existingFramework.Status = framework.Status;
            existingFramework.ReleaseDate = framework.ReleaseDate;
            existingFramework.CodeName = framework.CodeName;
        }

        dbContext.SaveChanges();

        var cacheRepository = scope.ServiceProvider.GetKeyedService<IFrameworkRepository>(MemoryCache.Constants.SERVICEKEY);
        if (cacheRepository is not null)
        {
            await cacheRepository.SaveAsync(framework, cancellationToken);
        }
    }

    public async Task<IReadOnlyCollection<Framework>> SearchAsync(FrameworkSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        return await dbContext.Frameworks.AsQueryable()
                        .ApplySearchCriteria(searchCriteria)
                        .ApplyPagination(a => a.Name, skip, take)
                        .ToArrayAsync(cancellationToken);
    }

    private async Task<bool> ExistsNoCacheAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        return await dbContext.Frameworks.AnyAsync(f => f.Name.Equals(name) && f.Version.Equals(version), cancellationToken);
    }

    private async Task<Framework?> TryGetByVersionNoCacheAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        return await dbContext.Frameworks.FindAsync([name, version], cancellationToken);
    }
}
