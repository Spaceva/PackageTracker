using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Fetcher;

internal class FetcherSettings : IRepeatedBackgroundSettings
{
    public TimeSpan TimeBetweenEachExecution { get; set; }

    public TrackedPackages Packages { get; set; } = default!;

    public class TrackedPackages
    {
        public ICollection<string> Npm { get; set; } = new List<string>();

        public ICollection<string> Nuget { get; set; } = new List<string>();
    }
}
