using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationScannedEventHandler(IApplicationsRepository applicationsRepository, IMediator mediator, ILogger<ApplicationScannedEventHandler> logger) : INotificationHandler<ApplicationScannedEvent>
{
    public async Task Handle(ApplicationScannedEvent notification, CancellationToken cancellationToken)
    {
        var scannedApplication = notification.Application;
        var existingApplication = await applicationsRepository.TryGetAsync(scannedApplication.Name, scannedApplication.Type, scannedApplication.RepositoryLink, cancellationToken);
        if (existingApplication is null)
        {
            var singleOrPluralBranch = scannedApplication.Branchs.Count > 1 ? "s" : string.Empty;
            logger.LogInformation("New {ApplicationType} application detected : '{ApplicationName}' ({BranchsCount} branch{SingleOrPluralBranch}).", scannedApplication.Type, scannedApplication.Name, scannedApplication.Branchs.Count, singleOrPluralBranch);
            await mediator.Send(new CreateApplicationCommand(notification), cancellationToken);
            return;
        }

        notification.Application.IsSoonDecommissioned = existingApplication.IsSoonDecommissioned;
        notification.Application.IsDeadLink = false;
        await mediator.Send(new UpdateApplicationCommand(notification), cancellationToken);
    }
}
