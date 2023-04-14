using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageVersionDeletedEventHandler : INotificationHandler<PackageVersionDeletedEvent>
{
    private readonly INotificationsRepository notificationsRepository;
    private readonly ILogger<PackageVersionDeletedEventHandler> logger;

    public PackageVersionDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageVersionDeletedEventHandler> logger)
    {
        this.notificationsRepository = notificationsRepository;
        this.logger = logger;
    }

    public async Task Handle(PackageVersionDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleted Package Version for {PackageName}: v{PackageVersionLabel}.", notification.PackageName, notification.PackageVersionLabel);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted Package Version for {notification.PackageName}: v{notification.PackageVersionLabel}",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageVersionDeletedEvent),
        });
    }
}
