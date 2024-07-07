using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Infrastructure.Http;

namespace PackageTracker.Scanner;

public class ScannerSettings : IRepeatedBackgroundSettings, IHttpProxy
{
    public TimeSpan TimeBetweenEachExecution { get; init; }

    public ICollection<TrackedApplication> Applications { get; init; } = default!;

    public string? ProxyUrl { get; init; }

    public class TrackedApplication
    {
        public string ScannerType { get; init; } = default!;

        public string RepositoryRootLink { get; init; } = default!;

        public string AccessToken { get; init; } = default!;

        public TimeSpan? TokenExpirationWarningThreshold { get; init; } = default!;

        public int MaximumConcurrencyCalls { get; init; } = 10;
    }
}
