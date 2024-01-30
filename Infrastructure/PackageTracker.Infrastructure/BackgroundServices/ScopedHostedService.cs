using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PackageTracker.Infrastructure.BackgroundServices;
internal class ScopedHostedService<TScopedBackgroundService>(IServiceScopeFactory serviceScopeFactory) : BackgroundService
    where TScopedBackgroundService : IScopedBackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<TScopedBackgroundService>();
        await scopedProcessingService.RunAsync(stoppingToken);
    }
}
