﻿namespace PackageTracker.Fetcher.PublicRegistries;

public static class ServiceCollectionExtensions
{
    public static IFetcherRegistrator AddPublicRegistriesFetchers(this IFetcherRegistrator services)
    {
        services.Register<PublicNugetServerFetcher>($"Public-{nameof(Domain.Package.Model.PackageType.Nuget)}");
        services.Register<PublicNpmServerFetcher>($"Public-{nameof(Domain.Package.Model.PackageType.Npm)}");
        services.Register<PublicPackagistServerFetcher>($"Public-{nameof(Domain.Package.Model.PackageType.Packagist)}");
        return services;
    }
}
