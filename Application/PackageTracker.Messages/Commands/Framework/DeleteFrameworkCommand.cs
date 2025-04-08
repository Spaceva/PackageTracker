using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class DeleteFrameworkCommand : IRequest
{
    public string Name { get; init; } = default!;

    public string Version { get; init; } = default!;
}
