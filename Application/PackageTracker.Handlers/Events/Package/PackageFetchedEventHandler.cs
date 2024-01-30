using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Package;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageFetchedEventHandler(IPackagesRepository packagesRepository, IMediator mediator, ILogger<PackageFetchedEventHandler> logger) : INotificationHandler<PackageFetchedEvent>
{
    public async Task Handle(PackageFetchedEvent notification, CancellationToken cancellationToken)
    {
        var fetchedPackage = notification.Package;
        var existingPackage = await packagesRepository.TryGetByNameAsync(fetchedPackage.Name, cancellationToken);
        if (existingPackage is null)
        {
            logger.LogInformation("New {PackageType} package detected : '{PackageName}'.", fetchedPackage.Type, fetchedPackage.Name);
            await mediator.Send(new CreatePackageCommand(notification), cancellationToken);
            return;
        }

        await mediator.Send(new UpdatePackageCommand(notification), cancellationToken);
    }
}
