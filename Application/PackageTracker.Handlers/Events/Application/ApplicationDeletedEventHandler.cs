using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class ApplicationDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationDeletedEventHandler> logger) : INotificationHandler<ApplicationDeletedEvent>
{
    public async Task Handle(ApplicationDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleted '{ApplicationName}' ({ApplicationType} application at {RepositoryLink}).", notification.Name, notification.Type, notification.RepositoryLink);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted '{notification.Name}' ({notification.Type} application at {notification.RepositoryLink}).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationDeletedEvent),
        }, cancellationToken);
    }
}
