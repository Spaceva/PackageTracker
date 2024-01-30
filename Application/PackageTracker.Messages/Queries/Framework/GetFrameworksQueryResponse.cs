using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Messages.Queries;

public class GetFrameworksQueryResponse
{
    public IReadOnlyCollection<Framework> Frameworks { get; init; } = Array.Empty<Framework>();
}
