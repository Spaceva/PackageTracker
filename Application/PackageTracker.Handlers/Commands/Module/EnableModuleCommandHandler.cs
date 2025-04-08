using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Modules;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers.Commands.Module;

internal class EnableModuleCommandHandler(IMediator mediator, IModuleManager moduleManager, ILogger<EnableModuleCommandHandler> logger) : IRequestHandler<EnableModuleCommand>
{
    public async Task Handle(EnableModuleCommand request, CancellationToken cancellationToken)
    {
        await moduleManager.EnableAsync(request.Name, cancellationToken);
        logger.LogInformation("Module {Name} has been enabled.", request.Name);
        await mediator.Publish(new ModuleEnabledEvent(request.Name), cancellationToken);
    }
}
