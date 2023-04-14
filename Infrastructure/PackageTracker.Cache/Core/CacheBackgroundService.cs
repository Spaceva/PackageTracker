using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Infrastructure.BackgroundServices;

namespace PackageTracker.Cache;

internal class CacheBackgroundService : RepeatedBackgroundService
{
    private readonly IEnumerable<ICache> caches;
    private readonly IOptionsMonitor<CacheSettings> cacheSettings;
    private bool isInit = false;

    public CacheBackgroundService(IEnumerable<ICache> caches, IOptionsMonitor<CacheSettings> cacheSettings, ILogger<CacheBackgroundService> logger)
        : base(logger)
    {
        this.caches = caches;
        this.cacheSettings = cacheSettings;
    }

    protected override Task CloseServiceAsync()
     => SaveAllAsync();

    protected override Task HandleRunErrorAsync(Exception exception, CancellationToken stoppingToken)
    {
        Logger.LogError(exception, "An error occured");
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        if (!isInit)
        {
            await LoadAllAsync();
            isInit = true;
            return;
        }

        await SaveAllAsync();
    }

    protected override TimeSpan TimeBetweenEachExecution()
        => cacheSettings.CurrentValue.TimeBetweenEachExecution;

    private async Task LoadAllAsync()
    {
        foreach (var cache in caches)
        {
            await cache.LoadAsync();
        }
    }

    private async Task SaveAllAsync()
    {
        foreach (var cache in caches)
        {
            await cache.SaveAsync();
        }
    }
}
