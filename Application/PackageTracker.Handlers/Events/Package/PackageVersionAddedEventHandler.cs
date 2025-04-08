using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class PackageVersionAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageVersionAddedEventHandler> logger) : INotificationHandler<PackageVersionAddedEvent>
{
    public async Task Handle(PackageVersionAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("New Package Version for {PackageName}: v{PackageVersionLabel}.", notification.PackageName, notification.PackageVersionLabel);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"New Package Version for {notification.PackageName}: v{notification.PackageVersionLabel}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageVersionAddedEvent),
        }, cancellationToken);
    }
}
