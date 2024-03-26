using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationModuleDbModel : ApplicationModule
{
    public ApplicationModuleDbModel(ApplicationModule applicationModule)
    {
        Name = applicationModule.Name;
        Framework = applicationModule.Framework;
        Packages = applicationModule.Packages;
        FrameworkVersion = applicationModule.FrameworkVersion;
    }

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
