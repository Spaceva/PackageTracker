using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Events;

public class ApplicationPackageVersionUpdatedEvent : PackageVersionMessage, INotification
{
    public string ApplicationName { get; init; } = default!;
    public string BranchName { get; init; } = default!;
    public string ModuleName { get; init; } = default!;
    public ApplicationType Type { get; init; } = default!;
    public string OldPackageVersionLabel { get; init; } = default!;
}
