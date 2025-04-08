namespace PackageTracker.Messages.Commands;

using PackageTracker.SharedKernel.Mediator;
using System;

public class ReadNotificationCommand : IRequest
{
    public Guid NotificationId { get; init; } = Guid.Empty;
}
