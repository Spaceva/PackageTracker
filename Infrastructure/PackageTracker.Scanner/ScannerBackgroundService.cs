using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Messages.Events;

namespace PackageTracker.Scanner;

internal class ScannerBackgroundService(IEnumerable<IApplicationsScanner> applicationsScanners, IOptionsMonitor<ScannerSettings> scannerSettings, IMediator mediator, ILogger<ScannerBackgroundService> logger) : RepeatedBackgroundService(logger, TimeSpan.FromSeconds(5))
{
    protected override Task CloseServiceAsync()
    {
        foreach (var applicationsScanner in applicationsScanners) { applicationsScanner.Dispose(); }
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        await Parallel.ForEachAsync(applicationsScanners, stoppingToken, ScanAsync);
    }

    private async ValueTask ScanAsync(IApplicationsScanner scanner, CancellationToken stoppingToken)
    {
        try
        {
            var deadLinksApplication = await scanner.FindDeadLinksAsync(stoppingToken);
            Logger.LogInformation("{Scanner} found {count} deadlink(s).", scanner.GetType().Name, deadLinksApplication.Count);
            await Parallel.ForEachAsync(deadLinksApplication, stoppingToken, PublishDeadLinkApplicationDetected);
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("{Scanner} stopped due to task cancellation.", scanner.GetType().Name);
            return;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("{Scanner} for deadlinks failed due to a {ExceptionType} : {ExceptionMessage}", scanner.GetType().Name, ex.GetType().Name, ex.Message);
        }

        try
        {
            var scannedApplications = await scanner.ScanRemoteAsync(stoppingToken);
            Logger.LogInformation("{Scanner} found {count} remote application(s).", scanner.GetType().Name, scannedApplications.Count);
            await Parallel.ForEachAsync(scannedApplications, stoppingToken, PublishApplicationScanned);
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("{Scanner} stopped due to task cancellation.", scanner.GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("{Scanner} for remote updates failed due to a {ExceptionType} : {ExceptionMessage}", scanner.GetType().Name, ex.GetType().Name, ex.Message);
        }
    }

    private async ValueTask PublishApplicationScanned(Application application, CancellationToken stoppingToken)
    {
        await mediator.Publish(new ApplicationScannedEvent(application), stoppingToken);
    }

    private async ValueTask PublishDeadLinkApplicationDetected(Application application, CancellationToken stoppingToken)
    {
        await mediator.Publish(new ApplicationDeadLinkDetectedEvent(application), stoppingToken);
    }

    protected override TimeSpan TimeBetweenEachExecution()
     => scannerSettings.CurrentValue.TimeBetweenEachExecution;
}