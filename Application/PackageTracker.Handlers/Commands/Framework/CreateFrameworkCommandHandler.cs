using MediatR;
using PackageTracker.Domain.Framework;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class CreateFrameworkCommandHandler(IMediator mediator, IFrameworkRepository frameworkRepository) : IRequestHandler<CreateFrameworkCommand>
{
    public async Task Handle(CreateFrameworkCommand request, CancellationToken cancellationToken)
    {
        var framework = request.Framework;

        await frameworkRepository.SaveAsync(framework, cancellationToken);

        var notification = new FrameworkAddedEvent(framework);

        await mediator.Publish(notification, cancellationToken);
    }
}
