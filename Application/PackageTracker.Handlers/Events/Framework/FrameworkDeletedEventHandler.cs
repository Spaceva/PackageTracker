using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class FrameworkDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<FrameworkDeletedEventHandler> logger) : INotificationHandler<FrameworkDeletedEvent>
{
    public async Task Handle(FrameworkDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleted {{FrameworkName}} {{FrameworkVersion}}.", notification.Name, notification.Version);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted {notification.Name} {notification.Version}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(FrameworkDeletedEvent),
        }, cancellationToken);
    }
}
