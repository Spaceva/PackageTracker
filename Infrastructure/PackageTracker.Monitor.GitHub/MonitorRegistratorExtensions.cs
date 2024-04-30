using PackageTracker.Monitor.GitHub.DotNet;
using PackageTracker.Monitor.GitHub.NodeJS;

namespace PackageTracker.Monitor.GitHub;

public static class MonitorRegistratorExtensions
{
    public static IMonitorRegistrator AddGitHubMonitors(this IMonitorRegistrator services)
    {
        services.Register<DotNetGitHubMonitor>();
        services.Register<NodeJSGitHubMonitor>();

        return services;
    }
}
