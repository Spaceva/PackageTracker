using PackageTracker.Domain.Framework;

namespace PackageTracker.Monitor;
public interface IMonitorRegistrator
{
    IMonitorRegistrator Register<TMonitor>() where TMonitor : class, IFrameworkMonitor;
    IMonitorRegistrator Register<TMonitor>(Func<IServiceProvider, TMonitor> factory) where TMonitor : class, IFrameworkMonitor;
    IMonitorRegistrator Register(Type type);
}
