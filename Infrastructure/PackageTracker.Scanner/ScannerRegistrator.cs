using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;
using static PackageTracker.Scanner.ScannerSettings;

namespace PackageTracker.Scanner;
internal class ScannerRegistrator(IServiceCollection services) : IScannerRegistrator
{
    public IScannerRegistrator Register<TScanner>(string trackerName, Func<IServiceProvider, ScannerSettings, TrackedApplication, IEnumerable<IApplicationModuleParser>, ILogger<TScanner>, IMediator, TScanner> factory)
         where TScanner : class, IApplicationsScanner
    {
        services.AddScoped<IApplicationsScanner, TScanner>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ScannerSettings>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var mediator = sp.GetRequiredService<IMediator>();
            var trackedApplication = settings.Value.Applications.SingleOrDefault(s => s.ScannerName.Equals(trackerName, StringComparison.OrdinalIgnoreCase)) ?? throw new UnknownScannerException();
            var parsers = sp.GetServices<IApplicationModuleParser>();
            return factory(sp, settings.Value, trackedApplication, parsers, loggerFactory.CreateLogger<TScanner>(), mediator);
        });
        return this;
    }
}
