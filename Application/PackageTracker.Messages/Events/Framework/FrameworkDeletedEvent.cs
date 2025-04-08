using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class FrameworkDeletedEvent : INotification
{
    public string Name { get; init; } = default!;

    public string Version { get; init; } = default!;
}
