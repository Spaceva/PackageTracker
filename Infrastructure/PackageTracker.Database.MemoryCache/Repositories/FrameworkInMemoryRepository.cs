﻿using Microsoft.Extensions.Caching.Memory;
using PackageTracker.Database.MemoryCache.Cloners;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MemoryCache.Repositories;
internal class FrameworkInMemoryRepository(IMemoryCache memoryCache, FrameworkCloner cloner) : IFrameworkRepository
{
    public Task<bool> ExistsAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var key = Key(name, version);
        return Task.FromResult(memoryCache.TryGetValue(key, out var _));
    }

    public Task DeleteByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var key = Key(name, version);
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<Framework> GetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var framework = await TryGetByVersionAsync(name, version, cancellationToken);
        return framework ?? throw new FrameworkNotFoundException();
    }

    public Task SaveAsync(Framework framework, CancellationToken cancellationToken = default)
    {
        memoryCache.Set(Key(framework), cloner.Clone(framework));
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Framework>> SearchAsync(FrameworkSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyCollection<Framework>>([]);
    }

    public Task<Framework?> TryGetByVersionAsync(string name, string version, CancellationToken cancellationToken = default)
    {
        var key = Key(name, version);
        memoryCache.TryGetValue(key, out Framework? framework);
        return Task.FromResult(cloner.Clone(framework));
    }

    private static string Key(string name, string version)
     => $"{nameof(FrameworkInMemoryRepository)}-{name}-{version}";

    private static string Key(Framework framework)
     => Key(framework.Name, framework.Version);
}
