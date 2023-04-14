using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Domain.Notifications;

public interface INotificationsRepository
{
    Task SaveAsync(Notification notification);

    Task SaveAsync(IReadOnlyCollection<Notification> notifications);

    Task MarkAsReadAsync(Guid notificationId);

    Task MarkAsReadAsync(IReadOnlyCollection<Guid> notificationIds);

    Task<IReadOnlyCollection<Notification>> GetAllNotificationsAsync();

    Task<IReadOnlyCollection<Notification>> GetAllUnreadAsync();
}
