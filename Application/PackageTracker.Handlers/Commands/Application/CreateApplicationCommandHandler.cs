using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class CreateApplicationCommandHandler(IMediator mediator, IApplicationsRepository applicationsRepository) : IRequestHandler<CreateApplicationCommand>
{
    public async Task Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = request.Application;

        await applicationsRepository.SaveAsync(application, cancellationToken);

        var notification = ApplicationAddedEvent(application);

        await mediator.Publish(notification, cancellationToken);
    }

    private static ApplicationAddedEvent ApplicationAddedEvent(Application application)
    {
        return new ApplicationAddedEvent { Name = application.Name, Path = application.Path, ApplicationType = application.Type, RepositoryType = application.RepositoryType };
    }
}
