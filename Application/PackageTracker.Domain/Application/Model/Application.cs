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
}
