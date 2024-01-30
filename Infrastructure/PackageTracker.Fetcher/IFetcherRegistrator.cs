using PackageTracker.Domain.Package;

namespace PackageTracker.Fetcher;
public interface IFetcherRegistrator
{
    IFetcherRegistrator Register<TFetcher>(string? key = null) where TFetcher : class, IPackagesFetcher;
    IFetcherRegistrator Register<TFetcher>(Func<IServiceProvider, TFetcher> factory, string? key = null) where TFetcher : class, IPackagesFetcher;
    IFetcherRegistrator Register(Type type, string? key = null);
}
