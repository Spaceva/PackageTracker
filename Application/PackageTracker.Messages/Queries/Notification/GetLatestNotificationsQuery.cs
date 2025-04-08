using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Queries;

public class GetLatestNotificationsQuery : IRequest<GetLatestNotificationsQueryResponse>
{
    public int? PageNumber { get; init; }

    public int? PageSize { get; init; }
}
