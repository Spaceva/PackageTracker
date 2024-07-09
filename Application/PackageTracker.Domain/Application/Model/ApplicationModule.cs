namespace PackageTracker.Domain.Application.Model;

using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;

public abstract class ApplicationModule
{
    public string Name { get; set; } = default!;

    public ICollection<ApplicationPackage> Packages { get; set; } = [];

    public Framework? Framework { get; set; }

    public string FrameworkVersion { get; set; } = default!;

    public abstract Task<Framework?> TryGetFrameworkAsync(IFrameworkRepository frameworkRepository, CancellationToken cancellationToken = default);

    public abstract Framework? TryGetFramework(IReadOnlyCollection<Framework> frameworks);
}
