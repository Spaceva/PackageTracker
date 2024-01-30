using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner;
internal class ScannerRegistrator(IServiceCollection services) : IScannerRegistrator
{
    public IScannerRegistrator Register<TScanner>()
         where TScanner : class, IApplicationsScanner
    {
        services.AddScoped<IApplicationsScanner, TScanner>();
        return this;
    }

    public IScannerRegistrator Register<TScanner>(Func<IServiceProvider, TScanner> factory)
         where TScanner : class, IApplicationsScanner
    {
        services.AddScoped<IApplicationsScanner, TScanner>(factory);
        return this;
    }

    public IScannerRegistrator Register(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.GetInterface(nameof(IApplicationsScanner)) is null)
        {
            throw new ArgumentException($"Type must implements {nameof(IApplicationsScanner)}");
        }

        services.AddScoped(typeof(IApplicationsScanner), type);
        return this;
    }
}
