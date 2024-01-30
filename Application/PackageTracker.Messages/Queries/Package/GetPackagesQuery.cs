using MediatR;
using PackageTracker.Domain.Package;

namespace PackageTracker.Messages.Queries;
public class GetPackagesQuery : IRequest<GetPackagesQueryResponse>
{
    public PackageSearchCriteria SearchCriteria { get; init; } = default!;
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
