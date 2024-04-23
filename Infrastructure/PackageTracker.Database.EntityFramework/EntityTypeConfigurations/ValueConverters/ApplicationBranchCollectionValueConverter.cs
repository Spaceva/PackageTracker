using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Application.Model;
using System.Text.Json;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationBranchCollectionValueConverter : ValueConverter<ICollection<ApplicationBranch>, string>
{
    private static readonly ApplicationModuleValueConverter applicationModuleValueConverter = new();
    private static ApplicationBranchModel ToModel(ApplicationBranch entity)
        => new()
        {
            ApplicationType = entity.GetType().ToApplicationType(),
            Name = entity.Name,
            Link = entity.RepositoryLink,
            Modules = entity.Modules.Select(x => (ApplicationModuleModel)applicationModuleValueConverter.ConvertToProvider(x)!).ToList(),
            LastCommit = entity.LastCommit
        };

    private static ApplicationBranch FromModel(ApplicationBranchModel model)
    {
        var applicationBranch = (ApplicationBranch)Activator.CreateInstance(model.ApplicationType.ToApplicationBranchType())!;
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
