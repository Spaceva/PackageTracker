using MediatR;
using PackageTracker.Domain.Packages.Model;

namespace PackageTracker.Messages.Events;

public class PackageCreatedEvent : INotification
{
    public Package Package { get; init; } = default!;
}
