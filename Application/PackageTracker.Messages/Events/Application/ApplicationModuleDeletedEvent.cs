﻿using PackageTracker.Domain.Application.Model;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ApplicationModuleDeletedEvent : INotification
{
    public string ApplicationName { get; init; } = default!;
    public string BranchName { get; init; } = default!;
    public string ModuleName { get; init; } = default!;
    public ApplicationType Type { get; init; } = default!;
}
