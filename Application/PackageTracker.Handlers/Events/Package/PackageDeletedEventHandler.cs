using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageDeletedEventHandler> logger) : INotificationHandler<PackageDeletedEvent>
{
    public async Task Handle(PackageDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removed Package {Type} : {Name} - {Link}.", notification.Type, notification.Name, notification.Link);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Removed Package {notification.Type} : {notification.Name} - {notification.Link}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageDeletedEvent),
        }, cancellationToken);
    }
}
