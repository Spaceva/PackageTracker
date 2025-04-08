using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;
public class ToggleModuleCommand : IRequest
{
    public string Name { get; set; } = default!;
}
