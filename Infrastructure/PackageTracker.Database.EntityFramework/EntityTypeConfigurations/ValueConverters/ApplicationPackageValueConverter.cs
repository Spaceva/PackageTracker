using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationPackageValueConverter : ValueConverter<ApplicationPackage, ApplicationPackageModel>
{
    private static ApplicationPackageModel ToModel(ApplicationPackage entity)
    {
        return new ApplicationPackageModel
        {
            Name = entity.PackageName,
            Version = entity.PackageVersion,
        };
    }

    private static ApplicationPackage FromModel(ApplicationPackageModel model)
    {
        return new ApplicationPackage
        {
            PackageName = model.Name,
            PackageVersion = model.Version
        };
    }

    public ApplicationPackageValueConverter()
        : base(x => ToModel(x), x => FromModel(x))
    {
    }
}
