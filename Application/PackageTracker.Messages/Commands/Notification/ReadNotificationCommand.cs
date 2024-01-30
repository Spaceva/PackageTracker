namespace PackageTracker.Messages.Commands;

using MediatR;
using System;

public class ReadNotificationCommand : IRequest
{
    public Guid NotificationId { get; init; } = Guid.Empty;
}
