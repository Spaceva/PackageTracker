using PackageTracker.Domain.Packages.Model;

namespace PackageTracker.Cache;

internal class PackagesFileContext : FileContext<Package, PackagesDb>
{
    public PackagesFileContext(string dbFileName)
        : base(dbFileName)
    {
    }

    protected override PackagesDb Db(IReadOnlyCollection<Package> entities)
     => new()
     {
         NpmPackages = entities.OfType<NpmPackage>().ToArray(),
         NugetPackages = entities.OfType<NugetPackage>().ToArray()
     };

    protected override IReadOnlyCollection<Package> Entities(PackagesDb db)
     => db!.NpmPackages.Cast<Package>().Union(db.NugetPackages).ToArray();
}