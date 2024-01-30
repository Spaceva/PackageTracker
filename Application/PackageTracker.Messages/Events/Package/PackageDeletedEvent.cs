using MediatR;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages.Events;

public class PackageDeletedEvent : INotification
{
    public string Name { get; init; } = default!;

    public PackageType Type { get; init; } = default!;

    public string Link { get; init; } = default!;
}
