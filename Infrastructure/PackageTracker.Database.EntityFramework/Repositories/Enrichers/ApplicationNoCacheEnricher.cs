using Microsoft.EntityFrameworkCore;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Infrastructure;

namespace PackageTracker.Database.EntityFramework.Repositories.Enrichers;
internal class ApplicationNoCacheEnricher(PackageTrackerDbContext dbContext, bool showOnlyTrackedPackages = false)
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

    private async Task<IDictionary<string, Package>> GetAllPackagesAsync(IReadOnlyCollection<string> packagesName, CancellationToken cancellationToken = default)
    {
        return await dbContext.Packages.AsNoTracking().Where(p => packagesName.Contains(p.Name)).ToDictionaryAsync(x => x.Name, cancellationToken: cancellationToken);
    }

    private async Task<IDictionary<(string Name, string Version), Framework>> GetAllFrameworksAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Frameworks.AsNoTracking().ToDictionaryAsync(f => (f.Name, f.Version), cancellationToken);
    }

    private void EnrichApplication(Application application, IDictionary<string, Package> packagesByName, IDictionary<(string Name, string Version), Framework> frameworks, CancellationToken cancellationToken = default)
    {
        application.Name = application.Name.Replace($" ({application.Type})", string.Empty);
        foreach (var branch in application.Branchs)
        {
            foreach (var module in branch.Modules)
            {
                module.Framework = MatchFramework(module, frameworks);
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

    private static Framework? MatchFramework(ApplicationModule module, IDictionary<(string Name, string Version), Framework> frameworks)
    {
        if (module is DotNetAssembly dotNetAssembly)
        {
            _ = frameworks.TryGetValue((DotNetAssembly.FrameworkName, dotNetAssembly.FrameworkVersion + ".0"), out var framework)
                || frameworks.TryGetValue((DotNetAssembly.FrameworkNameLegacy, dotNetAssembly.FrameworkVersion.Replace("Framework", string.Empty).Trim()), out framework)
                || frameworks.TryGetValue((DotNetAssembly.FrameworkNameStandard, dotNetAssembly.FrameworkVersion.Replace("Standard", string.Empty).Trim()), out framework);
            return framework;
        }

        if (module is AngularModule angularModule)
        {
            frameworks.TryGetValue((AngularModule.FrameworkName, angularModule.FrameworkVersion), out var framework);
            return framework;
        }

        if (module is PhpModule phpModule)
        {
            _ = frameworks.TryGetValue((PhpModule.FrameworkName, phpModule.FrameworkVersion), out var framework)
                || frameworks.TryGetValue((PhpModule.FrameworkName, new PackageVersion(phpModule.FrameworkVersion).ToStringMajorMinor()), out framework);
            return framework;
        }

        return null;
    }
}
