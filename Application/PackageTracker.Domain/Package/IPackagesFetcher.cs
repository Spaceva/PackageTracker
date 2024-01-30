namespace PackageTracker.Domain.Package;

using PackageTracker.Domain.Package.Model;

public interface IPackagesFetcher : IDisposable
{
    string RegistryUrl { get; }
    Task<IReadOnlyCollection<Package>> FetchAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Package>> FetchAsync(CancellationToken cancellationToken);
}
