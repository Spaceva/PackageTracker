using PackageTracker.Domain.Application.Model;
using System.Web;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationBranchDbModel()
{
    public ApplicationBranchDbModel(ApplicationBranch applicationBranch) : this()
    {
        RepositoryLink = HttpUtility.UrlEncode(applicationBranch.RepositoryLink);
        LastCommit = applicationBranch.LastCommit;
        Modules = [.. applicationBranch.Modules.Select(m => new ApplicationModuleDbModel(m))];
        Name = applicationBranch.Name;
    }
    public string? Name { get; set; }

    public ICollection<ApplicationModuleDbModel>? Modules { get; set; }

    public string? RepositoryLink { get; set; }

    public DateTime? LastCommit { get; set; }

    internal ApplicationBranch ToDomain(ApplicationType applicationType)
    {
        ApplicationBranch applicationBranch = (ApplicationBranch)Activator.CreateInstance(applicationType.ToApplicationBranchType())!;
        applicationBranch.Name = Name!;
        applicationBranch.RepositoryLink = HttpUtility.UrlDecode(RepositoryLink);
        applicationBranch.LastCommit = LastCommit;
        applicationBranch.Modules = [.. Modules!.Select(m => m.ToDomain(applicationType))];
        return applicationBranch;
    }
}