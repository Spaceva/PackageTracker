using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Database.MongoDb.Repositories;
internal class NotificationsDbRepository(MongoDbContext dbContext, ILogger<NotificationsDbRepository> logger) : BaseDbRepository<NotificationDbModel>(dbContext, logger), INotificationsRepository
{
    public async Task<IReadOnlyCollection<Notification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var notificationsDb = await FindAsync(Filter.Empty, cancellationToken);
        return [.. notificationsDb.Select(n => n.ToDomain())];
    }

    public async Task<IReadOnlyCollection<Notification>> GetAllUnreadAsync(CancellationToken cancellationToken = default)
    {
        var notificationsDb = await FindAsync(Filter.Eq(n => n.IsRead, false), cancellationToken);
        return [.. notificationsDb.Select(n => n.ToDomain())];
    }

    public async Task MarkAllAsReadAsync(CancellationToken cancellationToken = default)
    {
        await Collection.UpdateManyAsync(Filter.Eq(n => n.IsRead, false), Builders<NotificationDbModel>.Update.Set(f => f.IsRead, true), cancellationToken: cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await Collection.UpdateOneAsync(Filter.Eq(n => n.NotificationId, id), Builders<NotificationDbModel>.Update.Set(f => f.IsRead, true), cancellationToken: cancellationToken);
    }

    public async Task MarkAsReadAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        var values = ids.Select(id => id.ToString());
        await Collection.UpdateManyAsync(Filter.In(nameof(NotificationDbModel.Id), values), Builders<NotificationDbModel>.Update.Set(f => f.IsRead, true), cancellationToken: cancellationToken);
    }

    public async Task SaveAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(Filter.Eq(n => n.NotificationId, notification.Id), new NotificationDbModel(notification), cancellationToken);
    }
}
