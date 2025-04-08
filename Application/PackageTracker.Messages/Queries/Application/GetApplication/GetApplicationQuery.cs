using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Queries;
public class GetApplicationQuery : IRequest<GetApplicationQueryResponse>
{
    public string Name { get; init; } = default!;

    public string RepositoryLink { get; init; } = default!;

    public ApplicationType Type { get; init; } = default!;
}
