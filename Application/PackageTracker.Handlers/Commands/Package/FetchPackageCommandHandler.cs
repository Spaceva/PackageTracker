using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Package;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class FetchPackageCommandHandler(IMediator mediator, IServiceProvider serviceProvider) : IRequestHandler<FetchPackageCommand>
{
    public async Task Handle(FetchPackageCommand request, CancellationToken cancellationToken)
    {
        var packageFetcher = serviceProvider.GetRequiredKeyedService<IPackagesFetcher>($"Public-{request.PackageType}");

        var packages = await packageFetcher.FetchAsync([request.PackageName], cancellationToken);

        var notifications = packages.Select(p => new PackageFetchedEvent(p));

        foreach (var notification in notifications)
        {
            await mediator.Publish(notification, cancellationToken);
        }
    }
}
