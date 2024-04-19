using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class PackageDbModel(PackageType type) : Package, IMongoEntity
{
    public PackageDbModel(Package package) : this(package.Type)
    {
        Name = package.Name;
        Versions = package.Versions;
        RegistryUrl = package.RegistryUrl;
        Link = package.Link;
    }

    public ObjectId? Id { get; set; }

    public override PackageType Type => type;

    public Package ToDomain()
    {
        var packageType = Type switch
        {
            PackageType.Nuget => typeof(NugetPackage),
            PackageType.Npm => typeof(NpmPackage),
            PackageType.Packagist => typeof(PackagistPackage),
            _ => throw new UnknownPackageTypeException()
        };

        Package domainPackage = (Package)Activator.CreateInstance(packageType)!;
        domainPackage.Name = Name;
        domainPackage.Versions = Versions;
        domainPackage.RegistryUrl = RegistryUrl;
        domainPackage.Link = Link;

        return domainPackage;
    }
}
