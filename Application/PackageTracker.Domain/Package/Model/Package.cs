namespace PackageTracker.Domain.Package.Model;

public abstract class Package
{
    private readonly PackageVersionComparer _packageVersionComparer = new();

    public string Name { get; set; } = default!;

    public ICollection<PackageVersion> Versions { get; set; } = [];

    public abstract PackageType Type { get; }

    public string RegistryUrl { get; set; } = default!;

    public string Link { get; set; } = default!;

    public string? LatestReleaseVersion
     => Versions.Where(v => v.IsRelease)
                .OrderDescendingUsing(_packageVersionComparer)
                .Select(v => v.ToString())
                .FirstOrDefault();

    public IReadOnlyCollection<string> VersionLabelsDescending()
     => [.. Versions.OrderDescendingUsing(_packageVersionComparer).Select(v => v.ToString())];

    public string? LatestVersion => VersionLabelsDescending().FirstOrDefault();

    public void ReplaceVersionsWith(IReadOnlyCollection<PackageVersion> versions)
    {
        Versions.Clear();
        foreach (var version in versions)
        {
            Versions.Add(version);
        }
    }
}
