using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;
public class EnableModuleCommand : IRequest
{
    public string Name { get; set; } = default!;
}
