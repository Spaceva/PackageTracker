using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Notifications.Exceptions;
using PackageTracker.Domain.Notifications.Model;
using System.Collections.Concurrent;

namespace PackageTracker.Infrastructure.Repositories;

public class NotificationsCacheRepository : CacheRepository<Notification>, INotificationsRepository
{
    private readonly ConcurrentDictionary<Guid, Notification> notifications = new();

    public Task SaveAsync(Notification notification)
    {
        Save(notification);
        return Task.CompletedTask;
    }

    public Task SaveAsync(IReadOnlyCollection<Notification> notifications)
    {
        Save(notifications);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Notification>> GetAllNotificationsAsync()
     => Task.FromResult(GetAll());

    public Task<IReadOnlyCollection<Notification>> GetAllUnreadAsync()
     => Task.FromResult(GetAllUnread());

    public Task MarkAsReadAsync(Guid notificationId)
    {
        MarkAsRead(notificationId);
        return Task.CompletedTask;
    }

    public Task MarkAsReadAsync(IReadOnlyCollection<Guid> notificationIds)
    {
        MarkAsRead(notificationIds);
        return Task.CompletedTask;
    }

    protected override Task<IReadOnlyCollection<Notification>> GetAllAsync()
        => GetAllUnreadAsync();

    protected override Task AddAsync(IReadOnlyCollection<Notification> entities)
        => SaveAsync(entities);

    private void Save(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        if (notifications.ContainsKey(notification.Id))
        {
            throw new NotificationAlreadyExistsException();
        }

        notifications.AddOrUpdate(notification.Id, notification, (id, oldNotification) => notification ?? oldNotification);
    }

    private void Save(IReadOnlyCollection<Notification> notifications)
    {
        ArgumentNullException.ThrowIfNull(notifications);
        foreach (var notification in notifications)
        {
            Save(notification);
        }
    }

    private IReadOnlyCollection<Notification> GetAll()
     => notifications.Values.ToArray();

    private IReadOnlyCollection<Notification> GetAllUnread()
     => notifications.Values.Where(n => !n.IsRead).ToArray();

    private void MarkAsRead(Guid notificationId)
    {
        if (!notifications.TryGetValue(notificationId, out Notification? notification))
        {
            throw new NotificationNotFoundException();
        }

        notification.IsRead = true;
    }

    private void MarkAsRead(IReadOnlyCollection<Guid> notificationIds)
    {
        foreach (var notificationId in notificationIds)
        {
            MarkAsRead(notificationId);
        }
    }
}
