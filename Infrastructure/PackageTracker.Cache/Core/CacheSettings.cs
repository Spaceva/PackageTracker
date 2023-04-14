using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Cache;

internal class CacheSettings : IRepeatedBackgroundSettings
{
    public TimeSpan TimeBetweenEachExecution { get; init; }
}
