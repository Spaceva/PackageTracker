using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Domain.Framework;

public class FrameworkSearchCriteria
{
    public string? Name { get; init; }

    public string? Version { get; init; }

    public IReadOnlyCollection<string>? Channel { get; init; }
    
    public IReadOnlyCollection<FrameworkStatus>? Status { get; init; }

    public string? CodeName { get; init; }

    public DateTime? ReleaseDateMinimum { get; init; }
    
    public DateTime? ReleaseDateMaximum { get; init; }

    public DateTime? EndOfLifeMinimum { get; init; }
    
    public DateTime? EndOfLifeMaximum { get; init; }
}