using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Application;
using MediatR;

namespace PackageTracker.Scanner.AzureDevOps;

public static class ServiceCollectionExtensions
{
    public static IScannerRegistrator AddAngularAzureDevOpsScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetRequiredService<IEnumerable<IApplicationModuleParser<AngularModule>>>();
            return new AngularAzureDevOpsScanner(trackedApplication, mediator, parsers, loggerFactory.CreateLogger<AngularAzureDevOpsScanner>(), settings.Value);
        });
    public static IScannerRegistrator AddDotNetAzureDevOpsScanner(this IScannerRegistrator services, string trackerName)
     => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetRequiredService<IEnumerable<IApplicationModuleParser<DotNetAssembly>>>();
            return new DotNetAzureDevOpsScanner(trackedApplication, mediator, parsers, loggerFactory.CreateLogger<DotNetAzureDevOpsScanner>(), settings.Value);
        });
}
