using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Database.EntityFramework.Extensions;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework;
internal class FrameworkDbRepository([FromKeyedServices(MemoryCache.Constants.SERVICEKEY)] IFrameworkRepository? cacheRepository, IServiceScopeFactory serviceScopeFactory) : IFrameworkRepository
{
    public async Task<Framework> GetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
        => await TryGetByVersionAsync(name, version, cancellationToken) ?? throw new FrameworkNotFoundException();

    public async Task<Framework?> TryGetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        return await dbContext.Frameworks.FindAsync([name, version], cancellationToken);
    }

    public async Task DeleteByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var existingFramework = await dbContext.Frameworks.FindAsync([name, version], cancellationToken) ?? throw new FrameworkNotFoundException();

        dbContext.Frameworks.Remove(existingFramework);
        dbContext.SaveChanges();
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
}
