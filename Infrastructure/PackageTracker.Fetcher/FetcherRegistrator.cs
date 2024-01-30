using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Package;

namespace PackageTracker.Fetcher;
internal class FetcherRegistrator(IServiceCollection services) : IFetcherRegistrator
{
    public IFetcherRegistrator Register<TFetcher>(string? key = null)
        where TFetcher : class, IPackagesFetcher
    {
        services.AddScoped<IPackagesFetcher, TFetcher>();
        if (!string.IsNullOrWhiteSpace(key))
        {
            services.AddKeyedScoped<IPackagesFetcher, TFetcher>(key);
        }

        return this;
    }

    public IFetcherRegistrator Register<TFetcher>(Func<IServiceProvider, TFetcher> factory, string? key = null)
        where TFetcher : class, IPackagesFetcher
    {
        services.AddScoped<IPackagesFetcher, TFetcher>(factory);
        if (!string.IsNullOrWhiteSpace(key))
        {
            services.AddKeyedScoped<IPackagesFetcher, TFetcher>(key, (sp, k) => factory(sp));
        }

        return this;
    }

    public IFetcherRegistrator Register(Type type, string? key = null)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.GetInterface(nameof(IPackagesFetcher)) is null)
        {
            throw new ArgumentException($"Type must implements {nameof(IPackagesFetcher)}");
        }

        services.AddScoped(typeof(IPackagesFetcher), type);
        if (!string.IsNullOrWhiteSpace(key))
        {
            services.AddKeyedScoped(typeof(IPackagesFetcher), key, type);
        }
        return this;
    }
}
