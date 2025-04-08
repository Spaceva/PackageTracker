using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Framework;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class UpdateFrameworkCommandHandler(IMediator mediator, IFrameworkRepository frameworkRepository, ILogger<UpdateFrameworkCommandHandler> logger) : BaseHandler<UpdateFrameworkCommand>(mediator)
{
    public override async Task Handle(UpdateFrameworkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var newFramework = request.Framework;

            var dbFramework = await frameworkRepository.GetByVersionAsync(newFramework.Name, newFramework.Version, cancellationToken);

            var message = new FrameworkUpdatedEvent { Name = dbFramework.Name, Version = dbFramework.Version, Channel = newFramework.Channel, CodeName = newFramework.CodeName, EndOfLife = newFramework.EndOfLife, ReleaseDate = newFramework.ReleaseDate, Status = newFramework.Status };

            await frameworkRepository.SaveAsync(newFramework, cancellationToken);

            await Mediator.Publish(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle {Command}", nameof(UpdateFrameworkCommand));
        }
    }
}
