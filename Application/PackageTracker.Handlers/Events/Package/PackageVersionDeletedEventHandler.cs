using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class PackageVersionDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageVersionDeletedEventHandler> logger) : INotificationHandler<PackageVersionDeletedEvent>
{
    public async Task Handle(PackageVersionDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleted Package Version for {PackageName}: v{PackageVersionLabel}.", notification.PackageName, notification.PackageVersionLabel);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted Package Version for {notification.PackageName}: v{notification.PackageVersionLabel}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageVersionDeletedEvent),
        }, cancellationToken);
    }
}
