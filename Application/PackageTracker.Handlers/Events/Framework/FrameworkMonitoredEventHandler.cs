using MediatR;
using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Framework;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class FrameworkMonitoredEventHandler(IFrameworkRepository frameworkRepository, IMediator mediator, ILogger<FrameworkMonitoredEventHandler> logger) : INotificationHandler<FrameworkMonitoredEvent>
{
    public async Task Handle(FrameworkMonitoredEvent notification, CancellationToken cancellationToken)
    {
        var framework = notification.Framework;
        var existingApplication = await frameworkRepository.TryGetByVersionAsync(framework.Name, framework.Version, cancellationToken);
        if (existingApplication is null)
        {
            logger.LogInformation($"New framework detected : {{FrameworkName}} {{FrameworkVersion}} (Channel {{FrameworkChannel}}).", framework.Name, framework.Version, framework.Channel);
            await mediator.Send(new CreateFrameworkCommand(notification.Framework), cancellationToken);
            return;
        }

        await mediator.Send(new UpdateFrameworkCommand(notification.Framework), cancellationToken);
    }
}
