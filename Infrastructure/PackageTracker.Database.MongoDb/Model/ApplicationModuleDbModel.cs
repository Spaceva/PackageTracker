using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationModuleDbModel()
{
    public ApplicationModuleDbModel(ApplicationModule applicationModule) : this()
    {
        Name = applicationModule.Name;
        Framework = applicationModule.Framework;
        Packages = [.. applicationModule.Packages.Select(p => new ApplicationPackageDbModel(p))];
        FrameworkVersion = applicationModule.FrameworkVersion;
    }

    public string Name { get; set; } = default!;

    public ICollection<ApplicationPackageDbModel> Packages { get; set; } = [];

    public Framework? Framework { get; set; }

    public string FrameworkVersion { get; set; } = default!;

    internal ApplicationModule ToDomain(ApplicationType applicationType)
    {
        var applicationModule = (ApplicationModule)Activator.CreateInstance(applicationType.ToApplicationModuleType())!;
        applicationModule.Name = Name;
        applicationModule.Packages = [.. Packages.Select(p => p.ToDomain())];
        applicationModule.FrameworkVersion = FrameworkVersion;
        return applicationModule;
    }
}
