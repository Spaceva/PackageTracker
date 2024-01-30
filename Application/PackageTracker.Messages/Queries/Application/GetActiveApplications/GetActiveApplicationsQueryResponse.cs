using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Queries;

public class GetActiveApplicationsQueryResponse
{
    public IReadOnlyCollection<Application> Applications { get; init; } = [];
}
