using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Scanner.Gitlab;

public static class ServiceCollectionExtensions
{
    public static IScannerRegistrator AddAngularGitlabScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<AngularModule>>();
            return new AngularGitlabScanner(trackedApplication, mediator, parsers, loggerFactory.CreateLogger<AngularGitlabScanner>());
        });

    public static IScannerRegistrator AddDotNetGitlabScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Unknown ScannerName.");
            var parsers = sp.GetServices<IApplicationModuleParser<DotNetAssembly>>();
            return new DotNetGitlabScanner(trackedApplication, mediator, parsers, loggerFactory.CreateLogger<DotNetGitlabScanner>());
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
            return new PhpGitlabScanner(trackedApplication, mediator, parsers, loggerFactory.CreateLogger<PhpGitlabScanner>());
        });
}
