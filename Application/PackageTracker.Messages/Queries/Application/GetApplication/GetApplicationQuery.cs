using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Queries;
public class GetApplicationQuery : IRequest<GetApplicationQueryResponse>
{
    public string Name { get; init; } = default!;

    public string RepositoryLink { get; init; } = default!;

    public ApplicationType Type { get; init; } = default!;
}
