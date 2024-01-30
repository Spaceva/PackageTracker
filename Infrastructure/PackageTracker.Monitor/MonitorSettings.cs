using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Infrastructure.Http;

namespace PackageTracker.Monitor;

public class MonitorSettings : IRepeatedBackgroundSettings, IHttpProxy
{
    public TimeSpan TimeBetweenEachExecution { get; init; }

    public IReadOnlyCollection<MonitoredFramework> Frameworks { get; init; } = default!;

    public string? ProxyUrl { get; init; }

    public class MonitoredFramework
    {
            public string MonitorName { get; init; } = default!;

            public string Url { get; init; } = default!;
    }
}
