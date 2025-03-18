using MediatR;

namespace PackageTracker.Messages.Events;

public class ModuleDisabledEvent(string name) : INotification
{
    public string Name => name;
}
