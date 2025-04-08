using PackageTracker.Domain.Framework;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Queries;
public class GetFrameworksQuery : IRequest<GetFrameworksQueryResponse>
{
    public FrameworkSearchCriteria SearchCriteria { get; init; } = default!;
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
