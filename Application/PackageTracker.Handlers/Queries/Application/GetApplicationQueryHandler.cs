using MediatR;
using PackageTracker.Domain.Application;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;

internal class GetApplicationQueryHandler(IApplicationsRepository applicationsRepository) : IRequestHandler<GetApplicationQuery, GetApplicationQueryResponse>
{
    public async Task<GetApplicationQueryResponse> Handle(GetApplicationQuery request, CancellationToken cancellationToken)
    {
        var application = await applicationsRepository.GetAsync(request.Name, request.Type, request.RepositoryLink, cancellationToken);
        return new GetApplicationQueryResponse { Application = application };
    }
}
