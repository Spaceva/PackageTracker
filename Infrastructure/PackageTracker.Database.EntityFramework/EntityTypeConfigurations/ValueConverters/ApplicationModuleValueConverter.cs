using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationModuleValueConverter : ValueConverter<ApplicationModule, ApplicationModuleModel>
{
    private static readonly ApplicationPackageValueConverter applicationPackageValueConverter = new();

    private static ApplicationModuleModel ToModel(ApplicationModule entity)
        => new()
        {
            ModuleType = entity.GetType().ToApplicationType(),
            Name = entity.Name,
            Packages = [.. entity.Packages.Select(x => (ApplicationPackageModel)applicationPackageValueConverter.ConvertToProvider(x)!)],
            FrameworkVersion = entity.FrameworkVersion,
        };

    private static ApplicationModule FromModel(ApplicationModuleModel model)
    {
        ApplicationModule applicationModule = (ApplicationModule)Activator.CreateInstance(model.ModuleType.ToApplicationModuleType())!;
        applicationModule.Name = model.Name;
        applicationModule.Packages = [.. model.Packages.Select(m => (ApplicationPackage)applicationPackageValueConverter.ConvertFromProvider(m)!)];
        applicationModule.FrameworkVersion = model.FrameworkVersion;
        return applicationModule;
    }

    public ApplicationModuleValueConverter()
        : base(x => ToModel(x), x => FromModel(x))
    {
    }
}
