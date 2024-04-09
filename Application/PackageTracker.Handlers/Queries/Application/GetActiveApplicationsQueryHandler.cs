using MediatR;
using PackageTracker.Domain.Application;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;

internal class GetActiveApplicationsQueryHandler(IApplicationsRepository applicationsRepository) : IRequestHandler<GetActiveApplicationsQuery, GetActiveApplicationsQueryResponse>
{
    public async Task<GetActiveApplicationsQueryResponse> Handle(GetActiveApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await applicationsRepository.SearchAsync(new ApplicationSearchCriteria
        {
            ApplicationTypes = [request.ApplicationType],
            ApplicationName = request.ApplicationName,
            LastCommitAfter = DateTime.UtcNow.AddMonths(-9),
        }, cancellationToken: cancellationToken);

        return new GetActiveApplicationsQueryResponse
        {
            Applications = [..applications.OrderBy(app => app.Name)]
        };
    }
}
