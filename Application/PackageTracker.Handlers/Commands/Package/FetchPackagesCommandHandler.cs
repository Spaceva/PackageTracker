using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Package;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class FetchPackagesCommandHandler(IMediator mediator, IServiceProvider serviceProvider) : IRequestHandler<FetchPackagesCommand>
{
    public async Task Handle(FetchPackagesCommand request, CancellationToken cancellationToken)
    {
        var packageFetcher = serviceProvider.GetRequiredKeyedService<IPackagesFetcher>($"Public-{request.PackageType}");

        var packages = await packageFetcher.FetchAsync(request.PackagesName, cancellationToken);

        var notifications = packages.Select(p => new PackageFetchedEvent(p));

        foreach (var notification in notifications)
        {
            await mediator.Publish(notification, cancellationToken);
        }
    }
}
