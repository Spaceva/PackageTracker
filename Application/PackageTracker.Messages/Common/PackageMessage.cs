using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Messages;

public abstract class PackageMessage
{
    protected PackageMessage() { }

    protected PackageMessage(PackageMessage packageMessage)
    {
        Package = packageMessage.Package;
    }

    public Package Package { get; init; } = default!;
}
