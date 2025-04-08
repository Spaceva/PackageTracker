using PackageTracker.Domain.Package;
using PackageTracker.Messages.Queries;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers.Queries;
internal class GetAllPackagesQueryHandler(IPackagesRepository packagesRepository) : IRequestHandler<GetPackagesQuery, GetPackagesQueryResponse>
{
    public async Task<GetPackagesQueryResponse> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.SearchCriteria);
        var packages = await packagesRepository.GetAllAsync(request.SearchCriteria.Name, request.SearchCriteria.Types, cancellationToken: cancellationToken);
        return new GetPackagesQueryResponse
        {
            Packages = [.. packages
                        .OrderBy(p => p.Type)
                        .ThenBy(p => p.Name)]
        };
    }
}
