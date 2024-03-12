using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;
internal class GetUnreadNotificationsQueryHandler(INotificationsRepository notificationsRepository) : IRequestHandler<GetUnreadNotificationsQuery, GetUnreadNotificationsQueryResponse>
{
    public async Task<GetUnreadNotificationsQueryResponse> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationsRepository.GetAllUnreadAsync(cancellationToken);
        return new GetUnreadNotificationsQueryResponse
        {
            Notifications = [.. notifications.OrderByDescending(n => n.DateTime)],
        };
    }
}
