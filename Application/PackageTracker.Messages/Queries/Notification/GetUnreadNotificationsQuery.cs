using MediatR;

namespace PackageTracker.Messages.Queries;

public class GetUnreadNotificationsQuery : IRequest<GetUnreadNotificationsQueryResponse>
{
}
