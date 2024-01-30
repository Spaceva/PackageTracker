using MediatR;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Messages.Commands;

public class CreateFrameworkCommand(Framework framework) : IRequest
{
    public Framework Framework => framework;
}
