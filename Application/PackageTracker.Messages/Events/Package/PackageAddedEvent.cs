using MediatR;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages.Events;

public class PackageAddedEvent : INotification
{
    public string Name { get; init; } = default!;

    public PackageType Type { get; init; } = default!;

    public string? LatestVersionLabel { get; init; } = default!;

    public bool NoReleasedVersion { get; init; } = false;

    public string Link { get; init; } = default!;
}
