using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework.Repositories.Enrichers;
internal class ApplicationWithCacheEnricher(IPackagesRepository packagesRepository, IFrameworkRepository frameworksRepository, bool showOnlyTrackedPackages = false)
{
    public async Task EnrichApplicationsAsync(IEnumerable<Application> applications, CancellationToken cancellationToken)
    {
        foreach (var application in applications)
        {
            await EnrichApplicationAsync(application, cancellationToken);
        }
    }

    public async Task EnrichApplicationAsync(Application application, CancellationToken cancellationToken = default)
    {
        application.Name = application.Name.Replace($" ({application.Type})", string.Empty);
        foreach (var branch in application.Branchs)
        {
            foreach (var module in branch.Modules)
            {
                module.Framework = await MatchFrameworkAsync(module, cancellationToken);
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

    private async Task<Framework?> MatchFrameworkAsync(ApplicationModule module, CancellationToken cancellationToken = default)
    {
        if (module is DotNetAssembly dotNetAssembly)
        {
            var framework = await frameworksRepository.TryGetByVersionAsync(DotNetAssembly.FrameworkName, dotNetAssembly.FrameworkVersion + ".0", cancellationToken);
            if (framework is not null)
            {
                return framework;
            }
            framework = await frameworksRepository.TryGetByVersionAsync(DotNetAssembly.FrameworkNameLegacy, dotNetAssembly.FrameworkVersion.Replace("Framework", string.Empty).Trim(), cancellationToken);
            if (framework is not null)
            {
                return framework;
            }

            return await frameworksRepository.TryGetByVersionAsync(DotNetAssembly.FrameworkNameStandard, dotNetAssembly.FrameworkVersion.Replace("Standard", string.Empty).Trim(), cancellationToken);
        }

        if (module is AngularModule angularModule)
        {
            return await frameworksRepository.TryGetByVersionAsync(AngularModule.FrameworkName, angularModule.FrameworkVersion, cancellationToken);
        }

        if (module is PhpModule phpModule)
        {
            return await frameworksRepository.TryGetByVersionAsync(PhpModule.FrameworkName, phpModule.FrameworkVersion, cancellationToken)
                            ?? await frameworksRepository.TryGetByVersionAsync(PhpModule.FrameworkName, new PackageVersion(phpModule.FrameworkVersion).ToStringMajorMinor(), cancellationToken);
        }

        return null;
    }
}
