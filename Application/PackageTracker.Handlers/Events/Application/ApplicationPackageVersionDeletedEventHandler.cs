using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationPackageVersionDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationPackageVersionDeletedEventHandler> logger) : INotificationHandler<ApplicationPackageVersionDeletedEvent>
{
    public async Task Handle(ApplicationPackageVersionDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleted '{{PackageName}}' package v{{PackageVersion}} from '{{ModuleName}}' module from '{{BranchName}}' branch from '{{ApplicationName}}' ({{ApplicationType}} application).", notification.PackageName, notification.PackageVersionLabel, notification.ModuleName, notification.BranchName, notification.ApplicationName, notification.Type);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted '{notification.PackageName}' package v{notification.PackageVersionLabel} from '{notification.ModuleName}' module from '{notification.BranchName}' branch from '{notification.ApplicationName}' ({notification.Type} application).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationPackageVersionDeletedEvent),
        }, cancellationToken);
    }
}
