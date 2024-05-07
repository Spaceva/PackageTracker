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

        Package package = (Package)Activator.CreateInstance(duplicatedObject.Type.ToPackageType())!;
        package.Name = duplicatedObject.Name;
        package.Versions = duplicatedObject.Versions;
        package.RegistryUrl = duplicatedObject.RegistryUrl;
        package.Link = duplicatedObject.Link;
        package.Versions = [.. duplicatedObject.Versions.Select(packageVersionCloner.Clone)];

        return package;
    }
}
