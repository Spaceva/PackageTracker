using MediatR;
using PackageTracker.Domain.Application;
using static PackageTracker.Scanner.ScannerSettings;

namespace PackageTracker.Scanner;
public interface IScannerRegistrator
{
    IApplicationsScanner Register(IServiceProvider serviceProvider, ScannerSettings scannerSettings, TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> parsers, IMediator mediator);
}
