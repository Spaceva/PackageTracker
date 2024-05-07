using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class ApplicationModuleAddedEventHandler(INotificationsRepository notificationsRepository, ILogger<ApplicationModuleAddedEventHandler> logger) : INotificationHandler<ApplicationModuleAddedEvent>
{
    public async Task Handle(ApplicationModuleAddedEvent notification, CancellationToken cancellationToken)
    {
        var singleOrPlural = notification.PackageVersions.Count > 1 ? "s" : string.Empty;
        logger.LogInformation("Added '{ModuleName}' module to '{ApplicationName}', on branch '{BranchName}' ({ApplicationType} application) - Counting {PackagesCount} package{SingleOrPlural}.", notification.ModuleName, notification.ApplicationName, notification.BranchName, notification.Type, notification.PackageVersions.Count, singleOrPlural);

        await notificationsRepository.SaveAsync(new Domain.Notifications.Model.Notification
        {
            Content = $"Added '{notification.ModuleName}' module to '{notification.ApplicationName}', on branch '{notification.BranchName}' ({notification.Type} application) - Counting {notification.PackageVersions.Count} package{singleOrPlural}.",
            DateTime = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            IsRead = false,
            Type = nameof(ApplicationModuleAddedEvent),
        }, cancellationToken);
    }
}
