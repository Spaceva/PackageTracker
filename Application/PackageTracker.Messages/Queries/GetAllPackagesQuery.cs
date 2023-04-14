using MediatR;

namespace PackageTracker.Messages.Queries;
public class GetAllPackagesQuery : IRequest<GetAllPackagesQueryResponse>
{
}
