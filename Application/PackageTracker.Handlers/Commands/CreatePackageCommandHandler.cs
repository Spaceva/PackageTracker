using MediatR;
using PackageTracker.Domain.Packages;
using PackageTracker.Domain.Packages.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand>
{
    private readonly IMediator mediator;
    private readonly IPackagesRepository packagesRepository;

    public CreatePackageCommandHandler(IMediator mediator, IPackagesRepository packagesRepository)
    {
        this.mediator = mediator;
        this.packagesRepository = packagesRepository;
    }

    public async Task<Unit> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = CreatePackage(request);

        await packagesRepository.AddPackageAsync(package);

        var notification = CreateEventWithLatestReleaseVersion(package);

        await mediator.Publish(notification, cancellationToken);

        return Unit.Value;
    }

    private static Package CreatePackage(CreatePackageCommand request)
     => request.Type switch
     {
         PackageType.Npm => new NpmPackage { Name = request.Name, Versions = request.Versions.ToList() },
         PackageType.Nuget => new NugetPackage { Name = request.Name, Versions = request.Versions.ToList() },
         _ => throw new InvalidOperationException(),
     };

    private static PackageAddedEvent CreateEventWithLatestReleaseVersion(Package package)
    {
        var latestReleaseVersion = package.LatestReleaseVersion;

        if (latestReleaseVersion is null)
        {
            return CreateEventWithLatestAnyVersion(package);
        }

        return new PackageAddedEvent { Name = package.Name, Type = package.Type, LatestVersionLabel = latestReleaseVersion, Link = package.Link };
    }

    private static PackageAddedEvent CreateEventWithLatestAnyVersion(Package package)
    => new() { Name = package.Name, Type = package.Type, LatestVersionLabel = package.LatestVersion, NoReleasedVersion = true, Link = package.Link };
}
