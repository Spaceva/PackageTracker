using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;
using MediatR;

namespace PackageTracker.Scanner.AzureDevOps;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddAzureDevOpsScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase)) ?? throw new UnknownScannerException();
            var parsers = sp.GetRequiredService<IEnumerable<IApplicationModuleParser>>();
            return new AzureDevOpsScanner(settings.Value, trackedApplication, parsers, loggerFactory.CreateLogger<AzureDevOpsScanner>(), mediator);
        });
}
