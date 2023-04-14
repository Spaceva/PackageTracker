using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Cache;

internal class NotificationsDb
{
    public IReadOnlyCollection<Notification> Notifications { get; set; } = default!;
}
