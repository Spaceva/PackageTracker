using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Packages.Model;
using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Messages.Events;

namespace PackageTracker.Fetcher;

internal class FetcherBackgroundService : RepeatedBackgroundService
{
    private readonly IOptionsMonitor<FetcherSettings> fetcherSettings;
    private readonly IMediator mediator;
    private readonly NpmRegistryHttpClient npmRegistryHttpClient;
    private readonly NugetRegistryHttpClient nugetRegistryHttpClient;

    public FetcherBackgroundService(IServiceProvider serviceProvider, IOptionsMonitor<FetcherSettings> fetcherSettings, ILogger<FetcherBackgroundService> logger)
        : base(logger)
    {
        this.fetcherSettings = fetcherSettings;
        this.mediator = serviceProvider.GetRequiredService<IMediator>();
        this.npmRegistryHttpClient = new NpmRegistryHttpClient(serviceProvider.GetRequiredService<ILogger<NpmRegistryHttpClient>>());
        this.nugetRegistryHttpClient = new NugetRegistryHttpClient(serviceProvider.GetRequiredService<ILogger<NugetRegistryHttpClient>>());
    }

    protected override Task CloseServiceAsync()
    {
        npmRegistryHttpClient.Dispose();
        nugetRegistryHttpClient.Dispose();
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        if(fetcherSettings.CurrentValue.Packages is null)
        {
            return;
        }

        await Parallel.ForEachAsync(fetcherSettings.CurrentValue.Packages.Npm, stoppingToken, FetchPackageNpm);
        await Parallel.ForEachAsync(fetcherSettings.CurrentValue.Packages.Nuget, stoppingToken, FetchPackageNuget);
    }

    protected override Task HandleRunErrorAsync(Exception exception, CancellationToken stoppingToken)
    {
        Logger.LogError(exception, "An error occured");
        return Task.CompletedTask;
    }

    protected override TimeSpan TimeBetweenEachExecution()
     => fetcherSettings.CurrentValue.TimeBetweenEachExecution;

    private async ValueTask FetchPackageNpm(string packageName, CancellationToken stoppingToken)
    {
        try
        {
            var packageVersions = await npmRegistryHttpClient.GetPackagesVersionsAsync(packageName, stoppingToken);
            await mediator.Publish(new PackageFetchedEvent { Type = PackageType.Npm, Name = packageName, Versions = packageVersions }, stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Skipped {package} : {exception}", packageName, ex.Message);
        }
    }

    private async ValueTask FetchPackageNuget(string packageName, CancellationToken stoppingToken)
    {
        try
        {
            var packageVersions = await nugetRegistryHttpClient.GetPackagesVersionsAsync(packageName, stoppingToken);
            await mediator.Publish(new PackageFetchedEvent { Type = PackageType.Nuget, Name = packageName, Versions = packageVersions }, stoppingToken);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Skipped {package} : {exception}", packageName, ex.Message);
        }
    }
}