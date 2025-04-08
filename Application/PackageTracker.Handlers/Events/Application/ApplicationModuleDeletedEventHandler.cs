using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class ApplicationModuleDeletedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationModuleDeletedEventHandler> logger) : INotificationHandler<ApplicationModuleDeletedEvent>
{
    public async Task Handle(ApplicationModuleDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleted '{ModuleName}' module from '{BranchName}' branch from '{ApplicationName}' ({ApplicationType} application).", notification.ModuleName, notification.BranchName, notification.ApplicationName, notification.Type);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Deleted '{notification.ModuleName}' module from '{notification.BranchName}' branch from '{notification.ApplicationName}' ({notification.Type} application).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationModuleDeletedEvent),
        }, cancellationToken);
    }
}
