using PackageTracker.Domain.Framework;

namespace PackageTracker.Domain.Application.Model;

public class DotNetAssembly : ApplicationModule
{
    public const string FrameworkName = ".NET";
    public const string FrameworkNameLegacy = ".NET Framework";
    public const string FrameworkNameStandard = ".NET Standard";

    public override async Task<Framework.Model.Framework?> TryGetFrameworkAsync(IFrameworkRepository frameworkRepository, CancellationToken cancellationToken = default)
     => await frameworkRepository.TryGetByVersionAsync(FrameworkName, FrameworkVersion + ".0", cancellationToken)
        ?? await frameworkRepository.TryGetByVersionAsync(FrameworkNameLegacy, FrameworkVersion.Replace("Framework", string.Empty).Trim(), cancellationToken)
        ?? await frameworkRepository.TryGetByVersionAsync(FrameworkNameStandard, FrameworkVersion.Replace("Standard", string.Empty).Trim(), cancellationToken);

    public override Framework.Model.Framework? TryGetFramework(IReadOnlyCollection<Framework.Model.Framework> frameworks)
    => frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkName, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(FrameworkVersion + ".0", StringComparison.OrdinalIgnoreCase))
       ?? frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkNameLegacy, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(FrameworkVersion.Replace("Framework", string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))
       ?? frameworks.FirstOrDefault(f => f.Name.Equals(FrameworkNameStandard, StringComparison.OrdinalIgnoreCase) && f.Version.Equals(FrameworkVersion.Replace("Standard", string.Empty).Trim(), StringComparison.OrdinalIgnoreCase));
}
