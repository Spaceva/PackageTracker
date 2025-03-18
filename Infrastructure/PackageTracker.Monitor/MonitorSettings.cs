using PackageTracker.Infrastructure.Http;
using PackageTracker.Infrastructure.Modules;

namespace PackageTracker.Monitor;

public class MonitorSettings : IModuleBackgroundSettings, IHttpProxy
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
