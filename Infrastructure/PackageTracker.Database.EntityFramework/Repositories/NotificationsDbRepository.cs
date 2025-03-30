using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Notifications.Exceptions;
using PackageTracker.Domain.Notifications.Model;

namespace PackageTracker.Database.EntityFramework;
internal class NotificationsDbRepository(IServiceScopeFactory serviceScopeFactory) : INotificationsRepository
{
    public async Task<IReadOnlyCollection<Notification>> GetAllAsync(CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await dbContext.Notifications.AsNoTracking().ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Notification>> GetAllUnreadAsync(CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        return await dbContext.Notifications.AsNoTracking().Where(n => !n.IsRead).ToArrayAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var notification = await GetNotificationAsync(id, dbContext, cancellationToken);

        notification.IsRead = true;

        dbContext.SaveChanges();
    }

    public async Task MarkAsReadAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var notificationsCount = await dbContext.Notifications.Where(n => ids.Contains(n.Id)).CountAsync(cancellationToken);
        if (notificationsCount < ids.Count)
        {
            throw new NotificationNotFoundException();
        }

        if (!dbContext.Database.IsInMemory())
        {
            await dbContext.Notifications.Where(n => ids.Contains(n.Id)).ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, n => true), cancellationToken: cancellationToken);
            return;
        }

        var notifications = await dbContext.Notifications.Where(n => ids.Contains(n.Id)).ToArrayAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return;
    }

    public async Task MarkAllAsReadAsync(CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        if (!dbContext.Database.IsInMemory())
        {
            await dbContext.Notifications.ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, n => true), cancellationToken: cancellationToken);
            return;
        }
     
        var notifications = await dbContext.Notifications.ToArrayAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(Notification notification, CancellationToken cancellationToken = default!)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var notificationFromDb = await dbContext.Notifications.FindAsync([notification.Id], cancellationToken);

        PreSave(notification, notificationFromDb, dbContext);

        dbContext.SaveChanges();
    }

    private static void PreSave(Notification notification, Notification? notificationFromDb, PackageTrackerDbContext dbContext)
    {
        if (notificationFromDb is null)
        {
            dbContext.Notifications.Add(notification);
            return;
        }

        notificationFromDb.IsRead = notification.IsRead;
        notificationFromDb.Content = notification.Content;
        notificationFromDb.DateTime = notification.DateTime;
    }

    private static async Task<Notification> GetNotificationAsync(Guid id, PackageTrackerDbContext dbContext, CancellationToken cancellationToken = default!)
    {
        return await dbContext.Notifications.FindAsync([id], cancellationToken) ?? throw new NotificationNotFoundException();
    }
}
