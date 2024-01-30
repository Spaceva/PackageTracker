﻿using MediatR;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Events;

public class ApplicationUpdatedEvent : INotification
{
    public string Name { get; init; } = default!;

    public string Path { get; init; } = default!;

    public ApplicationType ApplicationType { get; init; } = default!;

    public RepositoryType RepositoryType { get; init; } = default!;

    public bool IsSoonDecommissionned { get; init; }

    public bool IsDeadLink { get; init; }
}
