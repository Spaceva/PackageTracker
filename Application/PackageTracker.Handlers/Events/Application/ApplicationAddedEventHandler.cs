using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationAddedEventHandler> logger) : INotificationHandler<ApplicationAddedEvent>
{
    public async Task Handle(ApplicationAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Added {ApplicationPath} > '{ApplicationName}' ({ApplicationType} application). Source: {ApplicationRepositoryType}.", notification.Path, notification.Name, notification.ApplicationType, notification.RepositoryType);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Added {notification.Path} > '{notification.Name}' ({notification.ApplicationType} application). Source : {notification.RepositoryType}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationAddedEvent),
        }, cancellationToken);
    }
}
