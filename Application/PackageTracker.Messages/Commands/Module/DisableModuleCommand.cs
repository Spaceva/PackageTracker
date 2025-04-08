using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;
public class DisableModuleCommand : IRequest
{
    public string Name { get; set; } = default!;
}
