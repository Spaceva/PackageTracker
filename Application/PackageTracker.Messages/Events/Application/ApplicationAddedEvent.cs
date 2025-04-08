using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationAddedEvent : INotification
{
    public string Name { get; init; } = default!;

    public string Path { get; init; } = default!;

    public ApplicationType ApplicationType { get; init; } = default!;

    public RepositoryType RepositoryType { get; init; } = default!;
}
