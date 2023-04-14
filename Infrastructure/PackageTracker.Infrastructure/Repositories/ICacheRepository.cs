namespace PackageTracker.Infrastructure.Repositories;

public interface ICacheRepository
{
    Task SaveAsync(IReadOnlyCollection<object> entities);

    Task<IReadOnlyCollection<object>> LoadAsync();
}
