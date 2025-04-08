using PackageTracker.Domain.Framework.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class FrameworkUpdatedEvent : INotification
{
    public string Name { get; init; } = default!;

    public string Version { get; init; } = default!;

    public string Channel { get; init; } = default!;

    public string? CodeName { get; init; }

    public FrameworkStatus Status { get; init; }

    public DateTime? ReleaseDate { get; init; }

    public DateTime? EndOfLife { get; init; }
}
