namespace PackageTracker.Messages;

public abstract class DestructuredPackageVersionMessage
{
    public DestructuredPackageVersionMessage() { }

    public string PackageName { get; init; } = default!;

    public string PackageVersionLabel { get; init; } = default!;

    public string PackageLink { get; init; } = default!;
}
