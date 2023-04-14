namespace PackageTracker.Infrastructure.Repositories;

public abstract class CacheRepository<TEntity> : ICacheRepository
{
    protected abstract Task<IReadOnlyCollection<TEntity>> GetAllAsync();

    protected abstract Task AddAsync(IReadOnlyCollection<TEntity> entities);

    async Task<IReadOnlyCollection<object>> ICacheRepository.LoadAsync()
    {
        var entities = await GetAllAsync();
        return entities.Cast<object>().ToArray();
    }

    Task ICacheRepository.SaveAsync(IReadOnlyCollection<object> entities)
    {
        return AddAsync(entities.Cast<TEntity>().ToArray());
    }
}
