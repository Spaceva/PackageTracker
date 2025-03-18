using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Modules;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers.Commands.Module;

internal class ToggleModuleCommandHandler(IMediator mediator, IModuleManager moduleManager, ILogger<ToggleModuleCommandHandler> logger) : IRequestHandler<ToggleModuleCommand>
{
    public async Task Handle(ToggleModuleCommand request, CancellationToken cancellationToken)
    {
        var isEnabled = await moduleManager.ToggleAsync(request.Name, cancellationToken);
        INotification notification = isEnabled ? new ModuleEnabledEvent(request.Name) : new ModuleDisabledEvent(request.Name);

        logger.LogInformation("Module {Name} has been {Status}.", request.Name, isEnabled ? "enabled" : "disabled");
        await mediator.Publish(notification, cancellationToken);
    }
}
