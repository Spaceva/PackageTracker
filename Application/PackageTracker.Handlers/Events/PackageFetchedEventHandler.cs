using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Packages;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageFetchedEventHandler : INotificationHandler<PackageFetchedEvent>
{
    private readonly IPackagesRepository packagesRepository;
    private readonly IMediator mediator;
    private readonly ILogger<PackageFetchedEventHandler> logger;

    public PackageFetchedEventHandler(IPackagesRepository packagesRepository, IMediator mediator, ILogger<PackageFetchedEventHandler> logger)
    {
        this.packagesRepository = packagesRepository;
        this.mediator = mediator;
        this.logger = logger;
    }

    public async Task Handle(PackageFetchedEvent packageFetched, CancellationToken cancellationToken)
    {
        var existingPackage = await packagesRepository.TryGetByNameAsync(packageFetched.Name);
        if (existingPackage is null)
        {
            logger.LogInformation("New package detected.");
            await mediator.Send(new CreatePackageCommand(packageFetched), cancellationToken);
            return;
        }

        await mediator.Send(new UpdatePackageCommand { Name = packageFetched.Name, Versions = packageFetched.Versions }, cancellationToken);
    }
}
