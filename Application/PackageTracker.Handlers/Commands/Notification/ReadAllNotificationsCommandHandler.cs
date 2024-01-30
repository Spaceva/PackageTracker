using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Commands;

namespace PackageTracker.Handlers.Commands;
internal class ReadAllNotificationsCommandHandler(INotificationsRepository notificationsRepository) : IRequestHandler<ReadAllNotificationsCommand>
{
    public async Task Handle(ReadAllNotificationsCommand request, CancellationToken cancellationToken)
    {
        await notificationsRepository.MarkAllAsReadAsync(cancellationToken);
    }
}
