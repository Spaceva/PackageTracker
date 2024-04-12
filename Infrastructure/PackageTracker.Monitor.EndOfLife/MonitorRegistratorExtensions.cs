using System.Reflection;

namespace PackageTracker.Monitor.EndOfLife;

public static class MonitorRegistratorExtensions
{
    public static IMonitorRegistrator AddEndOfLifeMonitors(this IMonitorRegistrator services)
    {
        var currentAssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
        foreach (var type in currentAssemblyTypes.Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(EndOfLifeFrameworkMonitor))))
        {
            services.Register(type);
        }

        return services;
    }
}
