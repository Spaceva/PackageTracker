using MediatR;

namespace PackageTracker.Messages.Commands;
public class ToggleModuleCommand : IRequest
{
    public string Name { get; set; } = default!;
}
