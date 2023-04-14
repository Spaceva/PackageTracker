using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class PackageVersionAddedEventHandler : INotificationHandler<PackageVersionAddedEvent>
{
    private readonly INotificationsRepository notificationsRepository;
    private readonly ILogger<PackageVersionAddedEventHandler> logger;

    public PackageVersionAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<PackageVersionAddedEventHandler> logger)
    {
        this.notificationsRepository = notificationsRepository;
        this.logger = logger;
    }

    public async Task Handle(PackageVersionAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("New Package Version for {PackageName}: v{PackageVersionLabel}", notification.PackageName, notification.PackageVersionLabel);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"New Package Version for {notification.PackageName}: v{notification.PackageVersionLabel}",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(PackageVersionAddedEvent),
        });
    }
}
