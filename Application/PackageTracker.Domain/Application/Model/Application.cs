namespace PackageTracker.Domain.Application.Model;

public abstract class Application
{
    public string Name { get; set; } = default!;

    public string Path { get; set; } = default!;

    public string RepositoryLink { get; set; } = default!;

    public virtual ICollection<ApplicationBranch> Branchs { get; set; } = [];

    public abstract ApplicationType Type { get; }

    public RepositoryType RepositoryType { get; set; }

    public bool IsSoonDecommissioned { get; set; }

    public bool IsDeadLink { get; set; }

    public static Application From(string applicationName, string repositoryPath, string repositoryLink, IReadOnlyCollection<ApplicationBranch> applicationBranches, RepositoryType repositoryType)
    {
        var applicationType = applicationBranches.Select(p => p.GetType().ToApplicationType()).Max();
        var application = (Application)Activator.CreateInstance(applicationType.ToApplicationType())!;
        application.Name = applicationName;
        application.RepositoryLink = repositoryLink;
        application.Path = repositoryPath;
        application.Branchs = [.. applicationBranches];
        application.RepositoryType = repositoryType;
        return application;
    }
}
