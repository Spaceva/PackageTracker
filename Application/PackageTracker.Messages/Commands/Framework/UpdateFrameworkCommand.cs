using PackageTracker.Domain.Framework.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Commands;

public class UpdateFrameworkCommand(Framework framework) : IRequest
{
    public Framework Framework => framework;
}
