using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MemoryCache.Cloners;
internal class PackageVersionCloner : BaseCloner<PackageVersion>
{
    public override PackageVersion Clone(PackageVersion duplicatedObject)
    {
        return new PackageVersion(duplicatedObject.ToString());
    }
}
