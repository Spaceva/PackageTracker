using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationPackageVersionAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationPackageVersionAddedEventHandler> logger) : INotificationHandler<ApplicationPackageVersionAddedEvent>
{
    public async Task Handle(ApplicationPackageVersionAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Added '{{PackageName}}' package v{{PackageVersion}} to '{{ModuleName}}' module in '{{BranchName}}' branch, in '{{Application}}' ({{ApplicationType}} application).", notification.PackageName, notification.PackageVersionLabel, notification.ApplicationName, notification.BranchName, notification.ModuleName, notification.Type);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Added '{notification.PackageName}' package v{notification.PackageVersionLabel} to '{notification.ModuleName}' module in '{notification.BranchName}' branch, in '{notification.ApplicationName}' ({notification.Type} application).",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationPackageVersionAddedEvent),
        }, cancellationToken);
    }
}
