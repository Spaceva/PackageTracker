using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationBranchAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationBranchAddedEventHandler> logger) : INotificationHandler<ApplicationBranchAddedEvent>
{
    public async Task Handle(ApplicationBranchAddedEvent notification, CancellationToken cancellationToken)
    {
        var singleOrPlural = notification.Modules.Count > 1 ? "s" : string.Empty;
        logger.LogInformation("Added '{BranchName}' branch to '{ApplicationName}' ({ApplicationType} application) - Counting {ModulesCount} module{singleOrPlural}.", notification.BranchName, notification.ApplicationName, notification.Type, notification.Modules.Count, singleOrPlural);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Added '{notification.BranchName}' branch to '{notification.ApplicationName}' ({notification.Type} application) - Counting {notification.Modules.Count} module{singleOrPlural}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationBranchAddedEvent),
        }, cancellationToken);
    }
}
