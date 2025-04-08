using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class CreatePackageCommandHandler(IMediator mediator, IPackagesRepository packagesRepository) : IRequestHandler<CreatePackageCommand>
{
    public async Task Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = request.Package;

        await packagesRepository.AddAsync(package, cancellationToken);

        var notification = CreateNotification(package);

        await mediator.Publish(notification, cancellationToken);
    }

    private static PackageAddedEvent CreateNotification(Package package)
    {
        var latestReleaseVersion = package.LatestReleaseVersion;

        if (latestReleaseVersion is null)
        {
            return CreateEventWithLatestPreReleaseVersion(package);
        }

        return new PackageAddedEvent { Name = package.Name, Type = package.Type, LatestVersionLabel = latestReleaseVersion, Link = package.Link };
    }

    private static PackageAddedEvent CreateEventWithLatestPreReleaseVersion(Package package)
    => new() { Name = package.Name, Type = package.Type, LatestVersionLabel = package.LatestVersion, NoReleasedVersion = true, Link = package.Link };
}
