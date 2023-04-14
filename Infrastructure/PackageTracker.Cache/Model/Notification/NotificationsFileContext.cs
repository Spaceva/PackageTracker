using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Cache;

internal class NotificationsFileContext : FileContext<Notification, NotificationsDb>
{
    public NotificationsFileContext(string dbFileName)
        : base(dbFileName)
    {
    }

    protected override NotificationsDb Db(IReadOnlyCollection<Notification> entities)
     => new()
     {
         Notifications = entities,
     };

    protected override IReadOnlyCollection<Notification> Entities(NotificationsDb db)
     => db!.Notifications;
}