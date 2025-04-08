using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Commands;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers.Commands;

internal class ReadNotificationCommandHandler(INotificationsRepository notificationsRepository) : IRequestHandler<ReadNotificationCommand>
{
    public async Task Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        await notificationsRepository.MarkAsReadAsync(request.NotificationId, cancellationToken);
    }
}
