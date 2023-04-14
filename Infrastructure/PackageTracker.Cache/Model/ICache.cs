using PackageTracker.Infrastructure.Repositories;

namespace PackageTracker.Cache;

internal interface ICache
{
    ICacheRepository Repository { get; }

    IFileContext FileContext { get; }

    Task LoadAsync();

    Task SaveAsync();
}
