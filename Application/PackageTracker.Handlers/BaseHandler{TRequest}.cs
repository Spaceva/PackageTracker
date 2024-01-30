using MediatR;

namespace PackageTracker.Handlers;
internal abstract class BaseHandler<TRequest>(IMediator mediator) : NotificationsPublisher(mediator), IRequestHandler<TRequest>
        where TRequest : IRequest
{
    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}
