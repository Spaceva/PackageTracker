namespace PackageTracker.Handlers.Queries;

using MediatR;
using PackageTracker.Domain.Packages;
using PackageTracker.Messages.Queries;
using System;
using System.Threading.Tasks;

internal class GetPackageByNameQueryHandler : IRequestHandler<GetPackageByNameQuery, GetPackageByNameQueryResponse>
{
    private readonly IPackagesRepository packagesRepository;

    public GetPackageByNameQueryHandler(IPackagesRepository packagesRepository)
    {
        this.packagesRepository = packagesRepository;
    }

    public async Task<GetPackageByNameQueryResponse> Handle(GetPackageByNameQuery request, CancellationToken cancellationToken)
    {
        var package = await packagesRepository.GetByNameAsync(Uri.UnescapeDataString(request.PackageName));
        return new GetPackageByNameQueryResponse
        {
            Name = package.Name,
            LatestReleaseVersion = package.LatestReleaseVersion,
            LatestVersion = package.LatestVersion,
            Type = package.Type.ToString(),
            Versions = package.VersionLabelsDescending,
            Link = package.Link,
        };
    }
}
