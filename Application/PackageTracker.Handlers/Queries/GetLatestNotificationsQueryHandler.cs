namespace PackageTracker.Handlers.Queries;

using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Domain.Notifications.Model;
using PackageTracker.Messages.Queries;

internal class GetLatestNotificationsQueryHandler : IRequestHandler<GetLatestNotificationsQuery, GetLatestNotificationsQueryResponse>
{
    private readonly INotificationsRepository notificationsRepository;

    public GetLatestNotificationsQueryHandler(INotificationsRepository notificationsRepository)
    {
        this.notificationsRepository = notificationsRepository;
    }

    public async Task<GetLatestNotificationsQueryResponse> Handle(GetLatestNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = (await notificationsRepository.GetAllNotificationsAsync()) ?? Array.Empty<Notification>();
        var pageSize = request.PageSize ?? notifications.Count;
        var skippedItems = request.PageNumber.HasValue && request.PageSize.HasValue ? (request.PageNumber.Value - 1) * request.PageSize.Value : 0;
        return new GetLatestNotificationsQueryResponse
        {
            Notifications = notifications
                .OrderByDescending(n => n.DateTime)
                .Skip(skippedItems)
                .Take(pageSize)
                .Select(n => new GetLatestNotificationsQueryResponse.NotificationDto
                {
                    Content = n.Content,
                    DateTime = n.DateTime,
                    Id = n.Id,
                    IsRead = n.IsRead,
                    Type = n.Type,
                })
                .ToArray(),
        };
    }
}
