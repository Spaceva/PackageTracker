using MediatR;
using PackageTracker.Domain.Application;

namespace PackageTracker.Messages.Queries;
public class GetApplicationsQuery : IRequest<GetApplicationsQueryResponse>
{
    public ApplicationSearchCriteria SearchCriteria { get; init; } = default!;
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
