using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.ApplicationModuleParsers;

internal abstract class ApplicationModuleParser(IPackagesRepository packagesRepository, ILogger logger) : IApplicationModuleParser
{
    public abstract bool CanParse(string fileContent);

    public abstract bool IsModuleFile(string fileAbsolutePath);

    public abstract Task<ApplicationModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken);

    protected async Task<ApplicationPackage> ApplicationPackage(string name, string version, CancellationToken cancellationToken)
    {
        Package? package = null;
        PackageVersion? packageVersion = null;
        try
        {
            package = await packagesRepository.GetByNameAsync(name, cancellationToken);
            packageVersion = await packagesRepository.GetVersionAsync(name, version, cancellationToken);
            package.Versions.Clear();
            package.Versions.Add(packageVersion);
        }
        catch (PackageNotFoundException)
        {
            logger.LogDebug("Package {PackageName} not found among tracked packages.", name);
        }
        catch (PackageVersionNotFoundException)
        {
            logger.LogWarning("Package {PackageName} version {PackageVersion} not found among version of the tracked package.", name, version);
        }

        return new ApplicationPackage { PackageName = name, PackageVersion = version, TrackedPackage = package, TrackedPackageVersion = packageVersion };
    }

    protected static string NormalizeVersion(string version)
    {
        if (version.Count(c => c.Equals('.')) >= 3)
        {
            return string.Join('.', version.Split('.').Take(3));
        }

        if (version.Count(c => c.Equals('.')) == 1)
        {
            return $"{version}.0";
        }

        return version;
    }
}
