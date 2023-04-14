using MediatR;
using PackageTracker.Domain.Packages;
using PackageTracker.Domain.Packages.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class UpdatePackageCommandHandler : IRequestHandler<UpdatePackageCommand>
{
    private readonly IMediator mediator;
    private readonly IPackagesRepository packagesRepository;

    public UpdatePackageCommandHandler(IMediator mediator, IPackagesRepository packagesRepository)
    {
        this.mediator = mediator;
        this.packagesRepository = packagesRepository;
    }

    public async Task<Unit> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await packagesRepository.GetByNameAsync(request.Name);

        var oldVersions = package.Versions.ToArray();

        package.Versions = request.Versions.ToArray();

        await packagesRepository.UpdatePackageAsync(package);

        var newVersions = request.Versions.ExceptBy(oldVersions.Select(v => v.Label), v => v.Label);
        if (newVersions.Any())
        {
            await PublishAddedVersions(package.Name, package.Link, newVersions, cancellationToken);
        }

        var deletedVersions = oldVersions.ExceptBy(request.Versions.Select(v => v.Label), v => v.Label);
        if (deletedVersions.Any())
        {
            await PublishDeletedVersions(package.Name, package.Link, deletedVersions, cancellationToken);
        }

        return Unit.Value;
    }
    private Task PublishAddedVersions(string packageName, string packageLink, IEnumerable<PackageVersion> packageVersions, CancellationToken cancellationToken)
     => Parallel.ForEachAsync(packageVersions, cancellationToken, (packageVersion, token) => PublishAddedVersion(packageName, packageLink, packageVersion, token));

    private Task PublishDeletedVersions(string packageName, string packageLink, IEnumerable<PackageVersion> packageVersions, CancellationToken cancellationToken)
     => Parallel.ForEachAsync(packageVersions, cancellationToken, (packageVersion, token) => PublishDeletedVersion(packageName, packageLink, packageVersion, token));

    private async ValueTask PublishAddedVersion(string packageName, string packageLink, PackageVersion packageVersion, CancellationToken cancellationToken)
     => await mediator.Publish(new PackageVersionAddedEvent { PackageName = packageName, PackageVersionLabel = packageVersion.Label, PackageLink = packageLink }, cancellationToken);

    private async ValueTask PublishDeletedVersion(string packageName, string packageLink, PackageVersion packageVersion, CancellationToken cancellationToken)
     => await mediator.Publish(new PackageVersionDeletedEvent { PackageName = packageName, PackageVersionLabel = packageVersion.Label, PackageLink = packageLink }, cancellationToken);
}
