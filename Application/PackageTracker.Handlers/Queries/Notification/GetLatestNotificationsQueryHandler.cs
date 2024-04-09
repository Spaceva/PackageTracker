namespace PackageTracker.Handlers.Queries;

using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Queries;

internal class GetLatestNotificationsQueryHandler(INotificationsRepository notificationsRepository) : IRequestHandler<GetLatestNotificationsQuery, GetLatestNotificationsQueryResponse>
{
    public async Task<GetLatestNotificationsQueryResponse> Handle(GetLatestNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = (await notificationsRepository.GetAllAsync(cancellationToken)) ?? [];
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
