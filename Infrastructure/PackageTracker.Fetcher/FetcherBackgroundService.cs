using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure.BackgroundServices;
using PackageTracker.Messages.Events;

namespace PackageTracker.Fetcher;

internal class FetcherBackgroundService(IServiceProvider serviceProvider, IOptionsMonitor<FetcherSettings> fetcherSettings, IMediator mediator, IPackagesRepository packagesRepository, ILogger<FetcherBackgroundService> logger) : RepeatedBackgroundService(logger)
{
    private IEnumerable<IPackagesFetcher> PackagesFetchers => serviceProvider.GetServices<IPackagesFetcher>();

    protected override Task CloseServiceAsync()
    {
        foreach (var packagesFetcher in PackagesFetchers) { packagesFetcher.Dispose(); }
        return Task.CompletedTask;
    }

    protected override async Task RunIterationAsync(CancellationToken stoppingToken)
    {
        await Parallel.ForEachAsync(PackagesFetchers, stoppingToken, FetchAsync);

        var packagesFromDb = await packagesRepository.GetAllAsync(cancellationToken: stoppingToken);

        var npmPackageFetcher = serviceProvider.GetKeyedService<IPackagesFetcher>($"Public-{nameof(PackageType.Npm)}");
        if (npmPackageFetcher is not null)
        {
            var npmPackages = await npmPackageFetcher.FetchAsync([.. packagesFromDb.Where(p => p.Type == PackageType.Npm && p.RegistryUrl.Equals(npmPackageFetcher.RegistryUrl)).Select(p => p.Name)], stoppingToken);
            await Parallel.ForEachAsync(npmPackages, stoppingToken, PublishPackageFetched);
        }

        var nugetPackageFetcher = serviceProvider.GetKeyedService<IPackagesFetcher>($"Public-{nameof(PackageType.Nuget)}");
        if (nugetPackageFetcher is not null)
        {
            var nugetPackages = await nugetPackageFetcher.FetchAsync([.. packagesFromDb.Where(p => p.Type == PackageType.Nuget && p.RegistryUrl.Equals(nugetPackageFetcher.RegistryUrl)).Select(p => p.Name)], stoppingToken);
            await Parallel.ForEachAsync(nugetPackages, stoppingToken, PublishPackageFetched);
        }

        var packagistPackageFetcher = serviceProvider.GetKeyedService<IPackagesFetcher>($"Public-{nameof(PackageType.Packagist)}");
        if (packagistPackageFetcher is not null)
        {
            var packagistPackages = await packagistPackageFetcher.FetchAsync([.. packagesFromDb.Where(p => p.Type == PackageType.Packagist && p.RegistryUrl.Equals(packagistPackageFetcher.RegistryUrl)).Select(p => p.Name)], stoppingToken);
            await Parallel.ForEachAsync(packagistPackages, stoppingToken, PublishPackageFetched);
        }
    }

    private async ValueTask FetchAsync(IPackagesFetcher fetcher, CancellationToken token)
    {
        try
        {
            var packages = await fetcher.FetchAsync(token);
            await Parallel.ForEachAsync(packages, token, PublishPackageFetched);
        }
        catch (TaskCanceledException)
        {
            Logger.LogWarning("{Fetcher} stopped due to task cancellation.", fetcher.GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogWarning("{Fetcher} failed to fetch due to a {ExceptionType} : {ExceptionMessage}", fetcher.GetType().Name, ex.GetType().Name, ex.Message);
        }
    }

    private async ValueTask PublishPackageFetched(Package package, CancellationToken token)
    {
        await mediator.Publish(new PackageFetchedEvent(package), token);
    }

    protected override TimeSpan TimeBetweenEachExecution()
     => fetcherSettings.CurrentValue.TimeBetweenEachExecution;
}