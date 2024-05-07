using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class FrameworkAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<FrameworkAddedEventHandler> logger) : INotificationHandler<FrameworkAddedEvent>
{
    public async Task Handle(FrameworkAddedEvent notification, CancellationToken cancellationToken)
    {
        var framework = notification.Framework;
        logger.LogInformation("Added {FrameworkName} {FrameworkVersion} (Channel {FrameworkChannel}).", framework.Name, framework.Version, framework.Channel);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Added {framework.Name} {framework.Version} (Channel {framework.Channel}).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(FrameworkAddedEvent),
        }, cancellationToken);
    }
}
