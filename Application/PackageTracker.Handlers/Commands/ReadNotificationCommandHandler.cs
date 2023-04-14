using MediatR;
using PackageTracker.Domain.Notifications;
using PackageTracker.Messages.Commands;

namespace PackageTracker.Handlers.Commands;
internal class ReadNotificationCommandHandler : IRequestHandler<ReadNotificationCommand>
{
    private readonly INotificationsRepository notificationsRepository;

    public ReadNotificationCommandHandler(INotificationsRepository notificationsRepository)
    {
        this.notificationsRepository = notificationsRepository;
    }

    public async Task<Unit> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
    {
        await notificationsRepository.MarkAsReadAsync(request.NotificationId);

        return Unit.Value;
    }
}
