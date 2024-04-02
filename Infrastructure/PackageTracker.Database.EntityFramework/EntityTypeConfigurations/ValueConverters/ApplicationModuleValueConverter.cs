using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationModuleValueConverter : ValueConverter<ApplicationModule, ApplicationModuleModel>
{
    private static readonly ApplicationPackageValueConverter applicationPackageValueConverter = new();
    private static ApplicationModuleModel ToModel(ApplicationModule entity)
    {
        ApplicationType? applicationType = null;
        var mainFrameworkVersion = "N/A";
            mainFrameworkVersion = entity.FrameworkVersion;
        if (entity is AngularModule angularModule)
        {
            applicationType = ApplicationType.Angular;
        }
        else if (entity is DotNetAssembly dotNetAssembly)
        {
            applicationType = ApplicationType.DotNet;
        }
        else if (entity is PhpModule phpModule)
        {
            applicationType = ApplicationType.Php;
        }
        return new ApplicationModuleModel
        {
            ModuleType = applicationType ?? throw new ArgumentOutOfRangeException(nameof(entity)),
            Name = entity.Name,
            Packages = entity.Packages.Select(x => (ApplicationPackageModel)applicationPackageValueConverter.ConvertToProvider(x)!).ToList(),
            FrameworkVersion = mainFrameworkVersion,
        };
    }

    private static ApplicationModule FromModel(ApplicationModuleModel model)
    {
        var applicationModuleType = model.ModuleType switch
        {
            ApplicationType.Angular => typeof(AngularModule),
            ApplicationType.DotNet => typeof(DotNetAssembly),
            ApplicationType.Php => typeof(PhpModule),
            _ => throw new ArgumentOutOfRangeException(model.GetType().Name)
        };
        ApplicationModule applicationModule = (ApplicationModule)Activator.CreateInstance(applicationModuleType)!;
        applicationModule.Name = model.Name;
        applicationModule.Packages = model.Packages.Select(m => (ApplicationPackage)applicationPackageValueConverter.ConvertFromProvider(m)!).ToList();
        applicationModule.FrameworkVersion = model.FrameworkVersion;
        return applicationModule;
    }

    public ApplicationModuleValueConverter()
        : base(x => ToModel(x), x => FromModel(x))
    {
    }
}
