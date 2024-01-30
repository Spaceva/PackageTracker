namespace PackageTracker.Messages;

public abstract class PackageVersionMessage
{
    protected PackageVersionMessage() { }

    protected PackageVersionMessage(PackageVersionMessage packageVersionMessage)
    {
        PackageName = packageVersionMessage.PackageName;
        PackageVersionLabel = packageVersionMessage.PackageVersionLabel;
        PackageLink = packageVersionMessage.PackageLink;
    }

    public string PackageName { get; init; } = default!;

    public string PackageVersionLabel { get; init; } = default!;

    public string? PackageLink { get; init; }
}
