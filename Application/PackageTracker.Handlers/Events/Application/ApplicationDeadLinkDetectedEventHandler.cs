using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationDeadLinkDetectedEventHandler(IApplicationsRepository applicationsRepository, IMediator mediator, ILogger<ApplicationDeadLinkDetectedEventHandler> logger) : INotificationHandler<ApplicationDeadLinkDetectedEvent>
{
    public async Task Handle(ApplicationDeadLinkDetectedEvent notification, CancellationToken cancellationToken)
    {
        var scannedApplication = notification.Application;
        var existingApplication = await applicationsRepository.TryGetAsync(scannedApplication.Name, scannedApplication.Type, scannedApplication.RepositoryLink, cancellationToken);
        if (existingApplication is null)
        {
            logger.LogWarning("False Positive Deadlink application detected : '{ApplicationName}' ({ApplicationType}).", scannedApplication.Name, scannedApplication.Type);
            return;
        }

        logger.LogInformation("Deadlink application detected : '{ApplicationName}' ({ApplicationType}).", scannedApplication.Name, scannedApplication.Type);
        if (existingApplication.IsDeadLink)
        {
            await mediator.Send(new DeleteApplicationCommand() { Name = existingApplication.Name, RepositoryLink = existingApplication.RepositoryLink, Type = existingApplication.Type }, cancellationToken);
            return;
        }

        notification.Application.IsDeadLink = true;
        await mediator.Send(new UpdateApplicationCommand(notification), cancellationToken);
    }
}
