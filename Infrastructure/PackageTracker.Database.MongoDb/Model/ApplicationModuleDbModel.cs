using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationModuleDbModel
{
    public ApplicationModuleDbModel(ApplicationModule applicationModule)
    {
        Name = applicationModule.Name;
        Framework = applicationModule.Framework;
        Packages = applicationModule.Packages;
        FrameworkVersion = applicationModule.FrameworkVersion;
    }

    public string Name { get; set; } = default!;

    public ICollection<ApplicationPackage> Packages { get; set; } = [];

    public Framework? Framework { get; set; }

    public string FrameworkVersion { get; set; } = default!;

    internal ApplicationModule ToDomain(ApplicationType applicationType)
    {
        var applicationModuleType = applicationType switch
        {
            ApplicationType.Angular => typeof(AngularModule),
            ApplicationType.DotNet => typeof(DotNetAssembly),
            ApplicationType.Php => typeof(PhpModule),
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType))
        };
        ApplicationModule applicationModule = (ApplicationModule)Activator.CreateInstance(applicationModuleType)!;
        applicationModule.Name = Name;
        applicationModule.Packages = Packages;
        applicationModule.FrameworkVersion = FrameworkVersion;
        return applicationModule;
    }
}
