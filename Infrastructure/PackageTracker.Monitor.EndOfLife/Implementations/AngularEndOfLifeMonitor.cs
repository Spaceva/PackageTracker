using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Monitor.EndOfLife;
internal class AngularEndOfLifeMonitor(ILogger<AngularEndOfLifeMonitor> logger, IOptions<MonitorSettings> options, IPackagesRepository packagesRepository) : EndOfLifeFrameworkMonitor("angular", logger, options.Value)
{
    private const string AngularPackage = "@angular/cli";

    protected override async Task<IReadOnlyCollection<Framework>> ParseAsync(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements, CancellationToken cancellationToken)
    {
        var angularPackage = await packagesRepository.TryGetByNameAsync(AngularPackage, cancellationToken);
        if (angularPackage is null)
        {
            return Map(elements);
        }

        var minCycle = int.Parse(elements.MinBy(e => e.Cycle)!.Cycle);
        var maxCycle = int.Parse(elements.MaxBy(e => e.Cycle)!.Cycle);
        var frameworks = new List<Framework>();
        foreach (var version in angularPackage.Versions)
        {
            var matchingCycle = elements.SingleOrDefault(e => e.Cycle.Equals($"{version.Major}"));
            if (matchingCycle is null)
            {
                if (version.Major <= minCycle)
                {
                    frameworks.Add(new()
                    {
                        Channel = $"{version.Major}",
                        Name = AngularModule.FrameworkName,
                        Version = version.ToString(),
                        Status = FrameworkStatus.EndOfLife,
                    });
                }
                else if (version.Major > maxCycle)
                {
                    frameworks.Add(new()
                    {
                        Channel = $"{version.Major}",
                        Name = AngularModule.FrameworkName,
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
                Name = AngularModule.FrameworkName,
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
     => elements.Select(ToFramework).ToArray();

    private Framework ToFramework(EndOfLifeHttpResponseElement source)
    {
        var activeStatus = source.Support < DateTime.UtcNow ? FrameworkStatus.LongTermSupport : FrameworkStatus.Active;
        return new()
        {
            Channel = source.Cycle,
            EndOfLife = source.EndOfLife,
            Name = AngularModule.FrameworkName,
            ReleaseDate = source.ReleaseDate,
            Status = source.EndOfLife < DateTime.UtcNow ? FrameworkStatus.EndOfLife : activeStatus,
            Version = source.Cycle
        };
    }
}
