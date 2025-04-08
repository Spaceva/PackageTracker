using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationDeletedEvent : INotification
{
    public string Name { get; set; } = default!;

    public string RepositoryLink { get; set; } = default!;

    public ApplicationType Type { get; set; } = default!;
}
