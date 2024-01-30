using MediatR;
using PackageTracker.Domain.Framework;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class DeleteFrameworkCommandHandler(IMediator mediator, IFrameworkRepository frameworkRepository) : IRequestHandler<DeleteFrameworkCommand>
{
    public async Task Handle(DeleteFrameworkCommand request, CancellationToken cancellationToken)
    {
        await frameworkRepository.DeleteByVersionAsync(request.Name, request.Version, cancellationToken);

        var notification = new FrameworkDeletedEvent() { Name = request.Name, Version = request.Version };

        await mediator.Publish(notification, cancellationToken);
    }
}
