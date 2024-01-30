using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner;
public interface IScannerRegistrator
{
    IScannerRegistrator Register<TScanner>() where TScanner : class, IApplicationsScanner;
    IScannerRegistrator Register<TScanner>(Func<IServiceProvider, TScanner> factory) where TScanner : class, IApplicationsScanner;
    IScannerRegistrator Register(Type type);
}
