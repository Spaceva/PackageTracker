using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Modules;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers.Commands.Module;

internal class DisableModuleCommandHandler(IMediator mediator, IModuleManager moduleManager, ILogger<DisableModuleCommandHandler> logger) : IRequestHandler<DisableModuleCommand>
{
    public async Task Handle(DisableModuleCommand request, CancellationToken cancellationToken)
    {
        await moduleManager.DisableAsync(request.Name, cancellationToken);
        logger.LogInformation("Module {Name} has been disabled.", request.Name);
        await mediator.Publish(new ModuleDisabledEvent(request.Name), cancellationToken);
    }
}
