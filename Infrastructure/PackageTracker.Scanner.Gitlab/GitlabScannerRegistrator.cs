using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner.Gitlab;

internal class GitlabScannerRegistrator : IScannerRegistrator
{
    public IApplicationsScanner Register(IServiceProvider serviceProvider, ScannerSettings scannerSettings, ScannerSettings.TrackedApplication trackedApplication, IEnumerable<IApplicationModuleParser> parsers, IMediator mediator)
    {
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<GitlabScanner>();
        return new GitlabScanner(trackedApplication, parsers, logger, mediator);
    }
}
