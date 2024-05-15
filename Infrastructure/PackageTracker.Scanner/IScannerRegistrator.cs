using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using static PackageTracker.Scanner.ScannerSettings;

namespace PackageTracker.Scanner;
public interface IScannerRegistrator
{
    IScannerRegistrator Register<TScanner>(string trackerName, Func<IServiceProvider, ScannerSettings, TrackedApplication, IEnumerable<IApplicationModuleParser>, ILogger<TScanner>, IMediator, TScanner> factory)
             where TScanner : class, IApplicationsScanner;
}
