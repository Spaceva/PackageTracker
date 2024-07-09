using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package;

namespace PackageTracker.Database.Common.Enrichers;
public class ApplicationWithCacheEnricher(IPackagesRepository packagesRepository, IFrameworkRepository frameworkRepository, bool showOnlyTrackedPackages = false) : IApplicationEnricher
{
    public async Task EnrichApplicationAsync(Application application, CancellationToken cancellationToken = default)
    {
        application.Name = application.Name.Replace($" ({application.Type})", string.Empty);
        foreach (var branch in application.Branchs)
        {
            foreach (var module in branch.Modules)
            {
                module.Framework = await module.TryGetFrameworkAsync(frameworkRepository, cancellationToken);
                foreach (var package in module.Packages)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var packageFromDb = await packagesRepository.TryGetByNameAsync(package.PackageName, cancellationToken);
                    if (packageFromDb is null)
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

    public async Task EnrichApplicationsAsync(IEnumerable<Application> applications, CancellationToken cancellationToken)
    {
        foreach (var application in applications)
        {
            await EnrichApplicationAsync(application, cancellationToken);
        }
    }
}
