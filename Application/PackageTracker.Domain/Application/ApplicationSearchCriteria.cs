using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Domain.Application;

public class ApplicationSearchCriteria
{
    public string? ApplicationName { get; init; }

    public IReadOnlyCollection<ApplicationType>? ApplicationTypes { get; init; }

    public IReadOnlyCollection<RepositoryType>? RepositoryTypes { get; init; }

    public bool ShowOnlyTracked { get; init; }

    public DateTime? LastCommitAfter { get; init; }

    public DateTime? LastCommitBefore { get; init; }

    public bool ApplyCommitFilterOnAllBranchs { get; init; }

    public FrameworkStatus? FrameworkStatus { get; init; }

    public bool ApplyFrameworkStatusFilterOnAllModules { get; init; }

    public bool ShowSoonDecommissioned { get; init; }

    public bool ShowDeadLink { get; init; }
}