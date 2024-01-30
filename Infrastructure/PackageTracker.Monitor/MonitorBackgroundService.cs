using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Messages.Events;

namespace PackageTracker.Monitor;

internal class MonitorBackgroundService(IEnumerable<IFrameworkMonitor> frameworkMonitors, IOptionsMonitor<MonitorSettings> monitorSettings, IMediator mediator, ILogger<MonitorBackgroundService> logger) : RepeatedBackgroundService(logger)
{
    protected override Task CloseServiceAsync()
    {
        foreach (var frameworkMonitor in frameworkMonitors) { frameworkMonitor.Dispose(); }
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        await Parallel.ForEachAsync(frameworkMonitors, stoppingToken, MonitorAsync);
    }

    private async ValueTask MonitorAsync(IFrameworkMonitor monitor, CancellationToken token)
    {
        try
        {
            var frameworks = await monitor.MonitorAsync(token);
            await Parallel.ForEachAsync(frameworks, token, PublishFrameworkMonitored);
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("{Monitor} stopped due to task cancellation.", monitor.GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("{Monitor} failed to monitor due to a {ExceptionType} : {ExceptionMessage}", monitor.GetType().Name, ex.GetType().Name, ex.Message);
        }
    }

    private async ValueTask PublishFrameworkMonitored(Framework framework, CancellationToken token)
    {
        await mediator.Publish(new FrameworkMonitoredEvent(framework), token);
    }

    protected override TimeSpan TimeBetweenEachExecution()
     => monitorSettings.CurrentValue.TimeBetweenEachExecution;
}