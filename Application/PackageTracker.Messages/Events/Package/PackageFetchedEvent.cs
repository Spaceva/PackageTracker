﻿using MediatR;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages.Events;

public class PackageFetchedEvent : PackageMessage, INotification
{
    public PackageFetchedEvent(Package package)
    {
        Package = package;
    }
}
