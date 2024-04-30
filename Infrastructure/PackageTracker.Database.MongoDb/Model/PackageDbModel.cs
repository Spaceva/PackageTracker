using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
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
        Package domainPackage = (Package)Activator.CreateInstance(Type.ToPackageType())!;
        domainPackage.Name = Name;
        domainPackage.Versions = Versions;
        domainPackage.RegistryUrl = RegistryUrl;
        domainPackage.Link = Link;

        return domainPackage;
    }
}
