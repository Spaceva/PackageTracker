using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class NotificationDbModel() : IMongoEntity
{
    public NotificationDbModel(Notification notification) : this()
    {
        NotificationId = notification.Id;
        DateTime = notification.DateTime;
        Content = notification.Content;
        Type = notification.Type;
        IsRead = notification.IsRead;
    }

    public ObjectId? Id { get; set; }

    public Guid NotificationId { get; set; }

    public DateTime DateTime { get; set; }

    public string Content { get; set; } = default!;

    public string Type { get; set; } = default!;

    public bool IsRead { get; set; }

    public Notification ToDomain()
    {
        return new Notification
        {
            Id = NotificationId,
            Content = Content,
            DateTime = DateTime,
            IsRead = IsRead,
            Type = Type,
        };
    }
}
