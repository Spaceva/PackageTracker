using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Domain.Notifications;

public interface INotificationsRepository
{
    Task SaveAsync(Notification notification, CancellationToken cancellationToken = default!);

    Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default!);

    Task MarkAsReadAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default!);

    Task MarkAllAsReadAsync(CancellationToken cancellationToken = default!);

    Task<IReadOnlyCollection<Notification>> GetAllAsync(CancellationToken cancellationToken = default!);

    Task<IReadOnlyCollection<Notification>> GetAllUnreadAsync(CancellationToken cancellationToken = default!);
}
