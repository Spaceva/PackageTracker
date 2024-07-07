using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner.AzureDevOps;

internal class AzureDevOpsScannerRegistrator : IScannerRegistrator
{
    public IApplicationsScanner Register(IServiceProvider serviceProvider, ScannerSettings scannerSettings, ScannerSettings.TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> parsers, IMediator mediator)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<AzureDevOpsScanner>();
        return new AzureDevOpsScanner(scannerSettings, trackedApplication, parsers, logger, mediator);
    }
}
