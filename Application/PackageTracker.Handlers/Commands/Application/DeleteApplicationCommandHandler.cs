using MediatR;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;

namespace PackageTracker.Handlers;

internal class DeleteApplicationCommandHandler(IMediator mediator, IApplicationsRepository applicationsRepository) : IRequestHandler<DeleteApplicationCommand>
{
    public async Task Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!await applicationsRepository.ExistsAsync(request.Name, request.Type, request.RepositoryLink, cancellationToken))
        {
            throw new ApplicationNotFoundException();
        }

        await applicationsRepository.DeleteAsync(request.Name, request.Type, request.RepositoryLink, cancellationToken);

        var notification = ApplicationDeletedEvent(request);

        await mediator.Publish(notification, cancellationToken);
    }

    private static ApplicationDeletedEvent ApplicationDeletedEvent(DeleteApplicationCommand application)
    {
        return new ApplicationDeletedEvent { Name = application.Name, RepositoryLink = application.RepositoryLink, Type = application.Type };
    }
}
