using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Queries;
public class GetActiveApplicationsQuery : IRequest<GetActiveApplicationsQueryResponse>
{
    public string? ApplicationName { get; init; }

    public ApplicationType ApplicationType { get; init; }
}
