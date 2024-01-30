using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Application.Model;
using System.Text.Json;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationBranchCollectionValueConverter : ValueConverter<ICollection<ApplicationBranch>, string>
{
    private static readonly ApplicationModuleValueConverter applicationModuleValueConverter = new();
    private static ApplicationBranchModel ToModel(ApplicationBranch entity)
    {
        var applicationType = entity.GetType().Name switch
        {
            nameof(AngularApplicationBranch) => ApplicationType.Angular,
            nameof(DotNetApplicationBranch) => ApplicationType.DotNet,
            nameof(PhpApplicationBranch) => ApplicationType.Php,
            _ => throw new ArgumentOutOfRangeException(entity.GetType().Name)
        };

        return new ApplicationBranchModel
        {
            ApplicationType = applicationType,
            Name = entity.Name,
            Link = entity.RepositoryLink,
            Modules = entity.Modules.Select(x => (ApplicationModuleModel)applicationModuleValueConverter.ConvertToProvider(x)!).ToList(),
            LastCommit = entity.LastCommit
        };
    }

    private static ApplicationBranch FromModel(ApplicationBranchModel model)
    {
        var applicationBranchType = model.ApplicationType switch
        {
            ApplicationType.Angular => typeof(AngularApplicationBranch),
            ApplicationType.DotNet => typeof(DotNetApplicationBranch),
            ApplicationType.Php => typeof(PhpApplicationBranch),
            _ => throw new ArgumentOutOfRangeException(model.GetType().Name)
        };
        ApplicationBranch applicationBranch = (ApplicationBranch)Activator.CreateInstance(applicationBranchType)!;
        applicationBranch.Name = model.Name;
        applicationBranch.RepositoryLink = model.Link;
        applicationBranch.Modules = model.Modules.Select(m => (ApplicationModule)applicationModuleValueConverter.ConvertFromProvider(m)!).ToList();
        applicationBranch.LastCommit = model.LastCommit;
        return applicationBranch;
    }

    public ApplicationBranchCollectionValueConverter()
        : base(x => JsonSerializer.Serialize(x.Select(ToModel), Constants.JsonSerializerOptions), x => JsonSerializer.Deserialize<List<ApplicationBranchModel>>(x, Constants.JsonSerializerOptions)!.Select(FromModel).ToList())
    {

    }
}
