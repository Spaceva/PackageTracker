using PackageTracker.Domain.Packages.Model;

namespace PackageTracker.Messages;

public abstract class DestructuredPackageMessage
{
    public DestructuredPackageMessage() { }

    protected DestructuredPackageMessage(DestructuredPackageMessage packageMessage)
    {
        Name = packageMessage.Name;
        Type = packageMessage.Type;
        Versions = packageMessage.Versions;
    }

    public string Name { get; init; } = default!;

    public PackageType Type { get; init; } = default!;

    public IReadOnlyCollection<PackageVersion> Versions { get; init; } = default!;
}
