using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationBranchDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationBranchDeletedEventHandler> logger) : INotificationHandler<ApplicationBranchDeletedEvent>
{
    public async Task Handle(ApplicationBranchDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleted '{{BranchName}}' branch from '{{ApplicationName}}' ({{ApplicationType}} application).", notification.BranchName, notification.ApplicationName, notification.Type);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted '{notification.BranchName}' branch from '{notification.ApplicationName}' ({notification.Type} application).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationBranchDeletedEvent),
        }, cancellationToken);
    }
}
