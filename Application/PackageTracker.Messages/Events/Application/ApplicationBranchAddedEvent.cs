using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationBranchAddedEvent : INotification
{
    public string ApplicationName { get; init; } = default!;
    public string BranchName { get; init; } = default!;
    public ApplicationType Type { get; init; } = default!;
    public IReadOnlyCollection<ApplicationModuleAddedEvent> Modules { get; init; } = [];
}
