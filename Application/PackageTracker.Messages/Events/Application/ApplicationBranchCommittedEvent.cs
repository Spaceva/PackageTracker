using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Events;

public class ApplicationBranchCommittedEvent : INotification
{
    public string ApplicationName { get; init; } = default!;
    public string BranchName { get; init; } = default!;
    public ApplicationType Type { get; init; } = default!;
    public DateTime? CommitDate { get; init; } = default!;
}
