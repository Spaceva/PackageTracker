using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;
internal class GetUnreadNotificationsQueryHandler : IRequestHandler<GetUnreadNotificationsQuery, GetUnreadNotificationsQueryResponse>
{
    private readonly INotificationsRepository notificationsRepository;

    public GetUnreadNotificationsQueryHandler(INotificationsRepository notificationsRepository)
    {
        this.notificationsRepository = notificationsRepository;
    }

    public async Task<GetUnreadNotificationsQueryResponse> Handle(GetUnreadNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationsRepository.GetAllUnreadAsync();
        return new GetUnreadNotificationsQueryResponse
        {
            Notifications = notifications.OrderByDescending(n => n.DateTime).ToArray(),
        };
    }
}
