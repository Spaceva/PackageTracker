namespace PackageTracker.SharedKernel.Mediator;

public interface IRequestHandler<TRequest> where TRequest: IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken = default);
}
