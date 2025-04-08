using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Exceptions;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class DeleteFrameworkCommandHandler(IMediator mediator, IFrameworkRepository frameworkRepository) : IRequestHandler<DeleteFrameworkCommand>
{
    public async Task Handle(DeleteFrameworkCommand request, CancellationToken cancellationToken)
    {
        if (!await frameworkRepository.ExistsAsync(request.Name, request.Version, cancellationToken))
        {
            throw new FrameworkNotFoundException();
        }

        await frameworkRepository.DeleteByVersionAsync(request.Name, request.Version, cancellationToken);

        var notification = new FrameworkDeletedEvent() { Name = request.Name, Version = request.Version };

        await mediator.Publish(notification, cancellationToken);
    }
}
