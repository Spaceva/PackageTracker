using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Commands;

public class DeleteApplicationCommand : IRequest
{
    public string Name { get; init; } = default!;

    public string RepositoryLink { get; init; } = default!;

    public ApplicationType Type { get; init; } = default!;
}
