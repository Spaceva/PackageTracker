using MediatR;

namespace PackageTracker.Handlers;
internal abstract class NotificationsPublisher(IMediator mediator)
{
    protected IMediator Mediator => mediator;

    protected async Task NotifyParallelAsync(IReadOnlyCollection<INotification> messages, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(messages, cancellationToken, async (message, token) =>
        {
            await Mediator.Publish(message, token);
        });
    }
}
