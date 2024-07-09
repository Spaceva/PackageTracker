using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.Common.Enrichers;
public abstract class ApplicationNoCacheEnricher(bool showOnlyTrackedPackages = false) : IApplicationEnricher
{
    public async Task EnrichApplicationAsync(Application application, CancellationToken cancellationToken = default)
        => await EnrichApplicationsAsync([application], cancellationToken);

    public async Task EnrichApplicationsAsync(IEnumerable<Application> applications, CancellationToken cancellationToken)
    {
        var allPackagesName = applications.SelectMany(a => a.Branchs.SelectMany(b => b.Modules.SelectMany(b => b.Packages.Select(b => b.PackageName)))).Distinct().ToArray();
        var packages = await GetAllPackagesAsync(allPackagesName, cancellationToken);
        var frameworks = await GetAllFrameworksAsync(cancellationToken);
        foreach (var application in applications)
        {
            EnrichApplication(application, packages, frameworks, cancellationToken);
        }
    }

    protected abstract Task<IDictionary<string, Package>> GetAllPackagesAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken = default);

    protected abstract Task<IReadOnlyCollection<Framework>> GetAllFrameworksAsync(CancellationToken cancellationToken);

    private void EnrichApplication(Application application, IDictionary<string, Package> packagesByName, IReadOnlyCollection<Framework> frameworks, CancellationToken cancellationToken = default)
    {
        application.Name = application.Name.Replace($" ({application.Type})", string.Empty);
        foreach (var branch in application.Branchs)
        {
            foreach (var module in branch.Modules)
            {
                module.Framework = module.TryGetFramework(frameworks);
                foreach (var package in module.Packages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!packagesByName.TryGetValue(package.PackageName, out var packageFromDb))
                    {
                        continue;
                    }

                    package.TrackedPackage = packageFromDb;
                    package.TrackedPackageVersion = packageFromDb.Versions.FirstOrDefault(p => p.Equals(package.PackageVersion));
                }

                if (showOnlyTrackedPackages)
                {
                    module.Packages = [.. module.Packages.Where(p => p.IsPackageTracked)];
                }
            }
        }
    }
}
