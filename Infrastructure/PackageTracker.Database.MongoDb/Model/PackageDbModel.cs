using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Package.Exceptions;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class PackageDbModel(Package package) : IMongoEntity
{
    public ObjectId? Id { get; set; }

    public string Name { get; set; } = package.Name;

    public ICollection<PackageVersion> Versions { get; set; } = package.Versions;

    public string RegistryUrl { get; set; } = package.RegistryUrl;

    public string Link { get; set; } = package.Link;

    public PackageType Type { get; set; } = package.Type;

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
