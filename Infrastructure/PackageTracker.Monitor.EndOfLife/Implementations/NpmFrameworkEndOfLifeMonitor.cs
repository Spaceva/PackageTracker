using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using System.Globalization;

namespace PackageTracker.Monitor.EndOfLife;
internal abstract class NpmFrameworkEndOfLifeMonitor(string framework, ILogger logger, IOptions<MonitorSettings> options, IPackagesRepository packagesRepository) : EndOfLifeFrameworkMonitor(framework, logger, options.Value)
{
    protected abstract string FrameworkPackageName { get; }

    protected abstract string FrameworkName { get; }

    protected override async Task<IReadOnlyCollection<Framework>> ParseAsync(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements, CancellationToken cancellationToken)
    {
        var frameworkPackage = await packagesRepository.TryGetByNameAsync(FrameworkPackageName, cancellationToken);
        if (frameworkPackage is null)
        {
            return Map(elements);
        }

        var minCycle = Convert.ToInt32(Math.Truncate(float.Parse(elements.MinBy(e => e.Cycle)!.Cycle, CultureInfo.InvariantCulture)));
        var maxCycle = Convert.ToInt32(Math.Truncate(float.Parse(elements.MaxBy(e => e.Cycle)!.Cycle, CultureInfo.InvariantCulture)));
        var frameworks = new List<Framework>();
        foreach (var version in frameworkPackage.Versions)
        {
            var matchingCycle = elements.SingleOrDefault(e => e.Cycle.Equals($"{version.Major}"));
            if (matchingCycle is null)
            {
                if (version.Major <= minCycle)
                {
                    frameworks.Add(new()
                    {
                        Channel = $"{version.Major}",
                        Name = FrameworkName,
                        Version = version.ToString(),
                        Status = FrameworkStatus.EndOfLife,
                    });
                }
                else if (version.Major > maxCycle)
                {
                    frameworks.Add(new()
                    {
                        Channel = $"{version.Major}",
                        Name = FrameworkName,
                        Version = version.ToString(),
                        Status = FrameworkStatus.Preview,
                    });
                }
                continue;
            }

            var status = ComputeStatus(version, matchingCycle);

            frameworks.Add(new()
            {
                ReleaseDate = matchingCycle.ReleaseDate,
                Channel = matchingCycle.Cycle,
                EndOfLife = matchingCycle.EndOfLife,
                Name = FrameworkName,
                Version = version.ToString(),
                Status = status,
            });
        }

        return frameworks;
    }

    private static FrameworkStatus ComputeStatus(PackageVersion version, EndOfLifeHttpResponseElement matchingCycle)
    {
        if (matchingCycle.EndOfLife > DateTime.UtcNow && version.IsRelease)
        {
            return matchingCycle.Support < DateTime.UtcNow ? FrameworkStatus.LongTermSupport : FrameworkStatus.Active;
        }

        return matchingCycle.ReleaseDate > DateTime.UtcNow ? FrameworkStatus.Preview : FrameworkStatus.EndOfLife;
    }

    private Framework[] Map(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements)
     => [.. elements.Select(ToFramework)];

    private Framework ToFramework(EndOfLifeHttpResponseElement source)
    {
        var activeStatus = source.Support < DateTime.UtcNow ? FrameworkStatus.LongTermSupport : FrameworkStatus.Active;
        return new()
        {
            Channel = source.Cycle,
            EndOfLife = source.EndOfLife,
            Name = FrameworkName,
            ReleaseDate = source.ReleaseDate,
            Status = source.EndOfLife < DateTime.UtcNow ? FrameworkStatus.EndOfLife : activeStatus,
            Version = source.Cycle
        };
    }
}
