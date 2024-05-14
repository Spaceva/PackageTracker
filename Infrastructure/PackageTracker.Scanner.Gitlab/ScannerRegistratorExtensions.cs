using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;

namespace PackageTracker.Scanner.Gitlab;

public static class ScannerRegistratorExtensions
{
    public static IScannerRegistrator AddGitlabScanner(this IScannerRegistrator services, string trackerName)
    => services.Register(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase)) ?? throw new UnknownScannerException();
            var parsers = sp.GetServices<IApplicationModuleParser>();
            return new GitlabScanner(trackedApplication, parsers, loggerFactory.CreateLogger<GitlabScanner>(), mediator);
        });
}
