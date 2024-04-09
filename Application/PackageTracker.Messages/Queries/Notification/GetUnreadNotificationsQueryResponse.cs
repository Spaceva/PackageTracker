using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Messages.Queries;

public class GetUnreadNotificationsQueryResponse
{
    public IReadOnlyCollection<Notification> Notifications { get; init; } = [];
}
