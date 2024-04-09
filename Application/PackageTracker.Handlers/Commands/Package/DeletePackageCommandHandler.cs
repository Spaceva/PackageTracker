using MediatR;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class DeletePackageCommandHandler(IMediator mediator, IPackagesRepository packagesRepository) : IRequestHandler<DeletePackageCommand>
{
    public async Task Handle(DeletePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await packagesRepository.GetByNameAsync(request.Name, cancellationToken);

        await packagesRepository.DeleteByNameAsync(request.Name, cancellationToken);

        var notification = PackageDeletedEvent(package);

        await mediator.Publish(notification, cancellationToken);
    }

    private static PackageDeletedEvent PackageDeletedEvent(Package package)
    {
        return new PackageDeletedEvent { Name = package.Name, Type = package.Type, Link = package.Link };
    }
}
