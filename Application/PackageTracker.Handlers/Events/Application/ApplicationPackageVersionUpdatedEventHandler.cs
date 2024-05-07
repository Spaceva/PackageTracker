using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationPackageVersionUpdatedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationPackageVersionUpdatedEventHandler> logger) : INotificationHandler<ApplicationPackageVersionUpdatedEvent>
{
    public async Task Handle(ApplicationPackageVersionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updated '{PackageName}' package from v{OldPackageVersion} to v{NewPackageVersion} in '{ModuleName}' module, in '{BranchName}' branch, in '{Application}' ({ApplicationType} application).", notification.PackageName, notification.OldPackageVersionLabel, notification.PackageVersionLabel, notification.ApplicationName, notification.BranchName, notification.ModuleName, notification.Type);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Updated '{notification.PackageName}' package from v{notification.OldPackageVersionLabel} to v{notification.PackageVersionLabel} in '{notification.ModuleName}' module, in '{notification.BranchName}' branch, in '{notification.ApplicationName}' ({notification.Type} application).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationPackageVersionUpdatedEvent),
        }, cancellationToken);
    }
}
