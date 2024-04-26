using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MemoryCache.Cloners;
internal class PackageCloner(PackageVersionCloner packageVersionCloner) : BaseCloner<Package?>
{
    public override Package? Clone(Package? duplicatedObject)
    {
        if(duplicatedObject is null)
        {
            return null;
        }

        var packageType = duplicatedObject.Type switch
        {
            PackageType.Nuget => typeof(NugetPackage),
            PackageType.Npm => typeof(NpmPackage),
            PackageType.Packagist => typeof(PackagistPackage),
            PackageType.Java => typeof(JavaPackage),
            _ => throw new ArgumentOutOfRangeException(nameof(duplicatedObject))
        };

        Package package = (Package)Activator.CreateInstance(packageType)!;
        package.Name = duplicatedObject.Name;
        package.Versions = duplicatedObject.Versions;
        package.RegistryUrl = duplicatedObject.RegistryUrl;
        package.Link = duplicatedObject.Link;
        package.Versions = [.. duplicatedObject.Versions.Select(packageVersionCloner.Clone)];

        return package;
    }
}
