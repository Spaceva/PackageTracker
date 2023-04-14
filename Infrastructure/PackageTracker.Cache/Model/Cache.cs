using Microsoft.Extensions.Logging;
using PackageTracker.Infrastructure.Repositories;

namespace PackageTracker.Cache;
internal class Cache : ICache
{
    private readonly ILogger<Cache> logger;

    public ICacheRepository Repository { get; } = default!;

    public IFileContext FileContext { get; } = default!;

    public Cache(ICacheRepository repository, IFileContext fileContext, ILogger<Cache> logger)
    {
        Repository = repository;
        FileContext = fileContext;
        this.logger = logger;
    }

    public async Task LoadAsync()
    {
        var entities = await FileContext.ReadAsync();
        await Repository.SaveAsync(entities);
        logger.LogInformation("Loaded {Count} element(s) from {Name} file.", entities.Count, FileContext.GetType().Name);
    }

    public async Task SaveAsync()
    {
        var entities = await Repository.LoadAsync();
        await FileContext.WriteAsync(entities);
        logger.LogInformation("Saved {Count} element(s) into {Name} file.", entities.Count, FileContext.GetType().Name);
    }
}
