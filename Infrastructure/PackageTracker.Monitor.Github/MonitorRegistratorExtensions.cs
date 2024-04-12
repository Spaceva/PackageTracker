using PackageTracker.Monitor.Github.DotNet;
using PackageTracker.Monitor.Github.NodeJS;

namespace PackageTracker.Monitor.Github;

public static class MonitorRegistratorExtensions
{
    public static IMonitorRegistrator AddGithubMonitors(this IMonitorRegistrator services)
    {
        services.Register<DotNetGithubMonitor>();
        services.Register<NodeJSGithubMonitor>();

        return services;
    }
}
