namespace PackageTracker.Infrastructure.BackgroundServices;

public interface IScopedBackgroundService
{
    Task RunAsync(CancellationToken stoppingToken);
}
