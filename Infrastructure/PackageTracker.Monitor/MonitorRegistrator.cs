using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Framework;

namespace PackageTracker.Monitor;
internal class MonitorRegistrator(IServiceCollection services) : IMonitorRegistrator
{
    public IMonitorRegistrator Register<TMonitor>()
         where TMonitor : class, IFrameworkMonitor
    {
        services.AddScoped<IFrameworkMonitor, TMonitor>();
        return this;
    }

    public IMonitorRegistrator Register<TMonitor>(Func<IServiceProvider, TMonitor> factory)
         where TMonitor : class, IFrameworkMonitor
    {
        services.AddScoped<IFrameworkMonitor, TMonitor>(factory);
        return this;
    }

    public IMonitorRegistrator Register(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.GetInterface(nameof(IFrameworkMonitor)) is null)
        {
            throw new ArgumentException($"Type must implements {nameof(IFrameworkMonitor)}");
        }

        services.AddScoped(typeof(IFrameworkMonitor), type);
        return this;
    }
}
