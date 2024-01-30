using MediatR;
using PackageTracker.Domain.Application;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;

internal class GetApplicationsQueryHandler(IApplicationsRepository applicationsRepository) : IRequestHandler<GetApplicationsQuery, GetApplicationsQueryResponse>
{
    public async Task<GetApplicationsQueryResponse> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var take = request.PageSize;
        var skip = request.PageNumber;
        if (take is not null && skip is not null)
        {
            skip *= take.Value;
        }

        var applications = await applicationsRepository.SearchAsync(request.SearchCriteria, skip, take, cancellationToken);
        return new GetApplicationsQueryResponse
        {
            Applications = [.. applications.OrderBy(app => app.Type).ThenBy(app => app.Name)]
        };
    }
}
