using PackageTracker.Domain.Framework;

namespace PackageTracker.Domain.Application.Model;

public class ReactModule : ApplicationModule
{
    public const string FrameworkName = "React";

    public override async Task<Framework.Model.Framework?> TryGetFrameworkAsync(IFrameworkRepository frameworkRepository, CancellationToken cancellationToken = default)
    => await frameworkRepository.TryGetByVersionAsync(FrameworkName, FrameworkVersion, cancellationToken);

    public override Framework.Model.Framework? TryGetFramework(IReadOnlyCollection<Framework.Model.Framework> frameworks)
    => frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkName, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(FrameworkVersion, StringComparison.OrdinalIgnoreCase));
}
