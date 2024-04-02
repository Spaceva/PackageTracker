using PackageTracker.Domain.Application.Model;
using System.Web;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationBranchDbModel
{
    public ApplicationBranchDbModel(ApplicationBranch applicationBranch)
    {
        Name = applicationBranch.Name;
        RepositoryLink = HttpUtility.UrlEncode(applicationBranch.RepositoryLink);
        LastCommit = applicationBranch.LastCommit;
        Modules = [.. applicationBranch.Modules.Select(m => new ApplicationModuleDbModel(m))];
    }
    public string Name { get; set; } = default!;

    public ICollection<ApplicationModuleDbModel> Modules { get; set; } = [];

    public string? RepositoryLink { get; set; }

    public DateTime? LastCommit { get; set; }

    internal ApplicationBranch ToDomain(ApplicationType applicationType)
    {
        var applicationBranchType = applicationType switch
        {
            ApplicationType.Angular => typeof(AngularApplicationBranch),
            ApplicationType.DotNet => typeof(DotNetApplicationBranch),
            ApplicationType.Php => typeof(PhpApplicationBranch),
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType))
        };

        ApplicationBranch applicationBranch = (ApplicationBranch)Activator.CreateInstance(applicationBranchType)!;
        applicationBranch.Name = Name;
        applicationBranch.RepositoryLink = HttpUtility.UrlDecode(RepositoryLink);
        applicationBranch.LastCommit = LastCommit;
        applicationBranch.Modules = [.. Modules.Select(m => m.ToDomain(applicationType))];
        return applicationBranch;
    }
}