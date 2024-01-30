using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Queries;

public class GetApplicationsQueryResponse
{
    public IReadOnlyCollection<Application> Applications { get; init; } = [];
}
