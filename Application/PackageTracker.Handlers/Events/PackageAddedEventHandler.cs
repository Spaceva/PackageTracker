using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageAddedEventHandler : INotificationHandler<PackageAddedEvent>
{
    private readonly INotificationsRepository notificationsRepository;
    private readonly ILogger<PackageAddedEventHandler> logger;

    public PackageAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageAddedEventHandler> logger)
    {
        this.notificationsRepository = notificationsRepository;
        this.logger = logger;
    }

    public async Task Handle(PackageAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("New Package {Type} : {Name} - v{LatestVersionLabel} {Release}", notification.Type, notification.Name, notification.LatestVersionLabel, notification.NoReleasedVersion ? "[PRE-RELEASE]" : "[RELEASE]");

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"New Package {notification.Type} : {notification.Name} - v{notification.LatestVersionLabel} {(notification.NoReleasedVersion ? "[PRE-RELEASE]" : "[RELEASE]")}",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageAddedEvent),
        });
    }
}
