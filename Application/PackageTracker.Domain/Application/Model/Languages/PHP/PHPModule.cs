using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Domain.Application.Model;

public class PhpModule : ApplicationModule
{
    public const string FrameworkName = "PHP";
    public override async Task<Framework.Model.Framework?> TryGetFrameworkAsync(IFrameworkRepository frameworkRepository, CancellationToken cancellationToken = default)
    => await frameworkRepository.TryGetByVersionAsync(FrameworkName, FrameworkVersion, cancellationToken)
    ?? await frameworkRepository.TryGetByVersionAsync(FrameworkName, new PackageVersion(FrameworkVersion).ToStringMajorMinor(), cancellationToken);

    public override Framework.Model.Framework? TryGetFramework(IReadOnlyCollection<Framework.Model.Framework> frameworks)
    => frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkName, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(FrameworkVersion, StringComparison.OrdinalIgnoreCase))
    ?? frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkName, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(new PackageVersion(FrameworkVersion).ToStringMajorMinor(), StringComparison.OrdinalIgnoreCase));
}
