using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class ApplicationUpdatedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationUpdatedEventHandler> logger) : INotificationHandler<ApplicationUpdatedEvent>
{
    public async Task Handle(ApplicationUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updated {ApplicationPath} > '{ApplicationName}' ({ApplicationType} application), source: {ApplicationRepositoryType}. IsSoonDecommissionned = {IsSoonDecommissionned}, IsDeadLink = {IsDeadLink}", notification.Path, notification.Name, notification.ApplicationType, notification.RepositoryType, notification.IsSoonDecommissionned, notification.IsDeadLink);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Updated {notification.Path} > '{notification.Name}' ({notification.ApplicationType} application), source : {notification.RepositoryType}. IsSoonDecommissionned = {notification.IsSoonDecommissionned}, IsDeadLink = {notification.IsDeadLink}",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationUpdatedEvent),
        }, cancellationToken);
    }
}
