﻿using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Messages.Events;

public class ModuleEnabledEvent(string name) : INotification
{
    public string Name => name;
}
