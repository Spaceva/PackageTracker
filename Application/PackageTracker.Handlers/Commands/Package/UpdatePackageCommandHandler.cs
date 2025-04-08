using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class UpdatePackageCommandHandler(IMediator mediator, IPackagesRepository packagesRepository) : BaseHandler<UpdatePackageCommand>(mediator)
{
    public async override Task Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var newPackage = request.Package;

        var dbPackage = await packagesRepository.GetByNameAsync(newPackage.Name, cancellationToken);

        var messages = PrepareMessages(newPackage, dbPackage);

        if (messages.Length == 0)
        {
            return;
        }

        dbPackage.ReplaceVersionsWith([.. newPackage.Versions]);

        await packagesRepository.UpdateAsync(dbPackage, cancellationToken);

        await NotifyParallelAsync(messages, cancellationToken);
    }

    private static INotification[] PrepareMessages(Package newPackage, Package dbPackage)
    {
        var messages = new List<INotification>();
        var oldVersions = dbPackage.Versions.ToArray();
        var newVersions = newPackage.Versions.Except(oldVersions, new PackageVersionComparer());
        if (newVersions.Any())
        {
            messages.AddRange(newVersions.Select(packageVersion => PackageVersionAddedEvent(dbPackage.Name, dbPackage.Link, packageVersion)));
        }

        var deletedVersions = oldVersions.Except(newPackage.Versions, new PackageVersionComparer());
        if (deletedVersions.Any())
        {
            messages.AddRange(deletedVersions.Select(packageVersion => PackageVersionDeletedEvent(dbPackage.Name, dbPackage.Link, packageVersion)));
        }

        return [..messages];
    }

    private static PackageVersionAddedEvent PackageVersionAddedEvent(string packageName, string packageLink, PackageVersion packageVersion)
     => new() { PackageName = packageName, PackageVersionLabel = packageVersion.ToString(), PackageLink = packageLink };

    private static PackageVersionDeletedEvent PackageVersionDeletedEvent(string packageName, string packageLink, PackageVersion packageVersion)
     => new() { PackageName = packageName, PackageVersionLabel = packageVersion.ToString(), PackageLink = packageLink };
}
