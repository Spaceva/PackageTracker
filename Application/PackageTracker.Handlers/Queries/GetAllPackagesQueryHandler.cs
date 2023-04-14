using MediatR;
using PackageTracker.Domain.Packages;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;
internal class GetAllPackagesQueryHandler : IRequestHandler<GetAllPackagesQuery, GetAllPackagesQueryResponse>
{
    private readonly IPackagesRepository packagesRepository;

    public GetAllPackagesQueryHandler(IPackagesRepository packagesRepository)
    {
        this.packagesRepository = packagesRepository;
    }

    public async Task<GetAllPackagesQueryResponse> Handle(GetAllPackagesQuery request, CancellationToken cancellationToken)
    {
        var packages = await packagesRepository.GetAllPackagesAsync();
        return new GetAllPackagesQueryResponse
        {
            Packages = packages
                        .OrderBy(p => p.Type)
                        .ThenBy(p => p.Name)
                        .Select(p => new GetAllPackagesQueryResponse.PackageDto
                        {
                            Name = p.Name,
                            Type = p.Type.ToString(),
                            LatestVersion = p.LatestVersion,
                            LatestReleaseVersion = p.LatestReleaseVersion,
                            Link = p.Link,
                        })
                        .ToArray()
        };
    }
}
