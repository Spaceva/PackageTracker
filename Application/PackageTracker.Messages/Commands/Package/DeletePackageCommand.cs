using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class DeletePackageCommand : IRequest
{
    public string Name { get; init; } = default!;
}
