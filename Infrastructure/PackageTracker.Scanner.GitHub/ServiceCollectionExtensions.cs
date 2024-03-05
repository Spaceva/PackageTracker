using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Scanner.GitHub;

public static class ServiceCollectionExtensions
{
    public static IScannerRegistrator AddAngularGitHubScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<AngularModule>>();
            return null;
        });

    public static IScannerRegistrator AddDotNetGitHubScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<DotNetAssembly>>();
            return null;
        });

    public static IScannerRegistrator AddPhpGitlabScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<PhpModule>>();
            return null;
        });
}
