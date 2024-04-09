using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationPackageDbModel()
{
    public ApplicationPackageDbModel(ApplicationPackage applicationPackage) : this()
    {
        PackageName = applicationPackage.PackageName;
        PackageVersion = applicationPackage.PackageVersion;
    }

    public string PackageName { get; init; } = default!;

    public string PackageVersion { get; init; } = default!;

    internal ApplicationPackage ToDomain()
    {
        return new ApplicationPackage { PackageName = PackageName, PackageVersion = PackageVersion };
    }
}
