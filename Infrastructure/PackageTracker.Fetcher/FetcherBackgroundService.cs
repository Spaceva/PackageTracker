﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Modules;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure.Modules;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Fetcher;

internal class FetcherBackgroundService(IServiceProvider serviceProvider, IOptionsMonitor<FetcherSettings> fetcherSettings, IMediator mediator, IPackagesRepository packagesRepository, IModuleManager moduleManager, ILogger<FetcherBackgroundService> logger) : ModuleBackgroundService(logger, moduleManager, TimeSpan.FromSeconds(1))
{
    private IEnumerable<IPackagesFetcher> PackagesFetchers => serviceProvider.GetServices<IPackagesFetcher>();

    private readonly PackageType[] packagesType = [.. Enum.GetValues<PackageType>().OfType<PackageType>()];

    protected override string ModuleName => Constants.ModuleName;

    protected override Task CloseServiceAsync()
    {
        foreach (var packagesFetcher in PackagesFetchers) { packagesFetcher.Dispose(); }
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        await Parallel.ForEachAsync(PackagesFetchers, stoppingToken, FetchAsync);

        var packagesFromDb = await packagesRepository.GetAllAsync(cancellationToken: stoppingToken);

        await Parallel.ForEachAsync(packagesType, stoppingToken, (p, token) => FetchExistingPackageTypeAsync(p, packagesFromDb, token));
    }

    private async ValueTask FetchExistingPackageTypeAsync(PackageType packageType, IEnumerable<Package> packagesFromDb, CancellationToken stoppingToken)
    {
        var packageFetcher = serviceProvider.GetKeyedService<IPackagesFetcher>($"Public-{packageType}");
        if (packageFetcher is null)
        {
            Logger.LogInformation("No public package fetcher found for {PackageType}.", packageType);
            return;
        }

        var packages = await packageFetcher.FetchAsync([.. packagesFromDb.Where(p => p.Type == packageType && p.RegistryUrl.Equals(packageFetcher.RegistryUrl)).Select(p => p.Name)], stoppingToken);
        await Parallel.ForEachAsync(packages, stoppingToken, PublishPackageFetched);
    }

    private async ValueTask FetchAsync(IPackagesFetcher fetcher, CancellationToken stoppingToken)
    {
        try
        {
            var packages = await fetcher.FetchAsync(stoppingToken);
            await Parallel.ForEachAsync(packages, stoppingToken, PublishPackageFetched);
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogWarning(ex, "{Fetcher} stopped.", fetcher.GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "{Fetcher} failed to fetch due to {ExceptionType}", fetcher.GetType().Name, ex.GetType().Name);
        }
    }

    private async ValueTask PublishPackageFetched(Package package, CancellationToken stoppingToken)
    {
        await mediator.Publish(new PackageFetchedEvent(package), stoppingToken);
    }

    protected override TimeSpan TimeBetweenEachExecution()
     => fetcherSettings.CurrentValue.TimeBetweenEachExecution;
}