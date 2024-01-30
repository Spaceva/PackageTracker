using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Monitor.EndOfLife;
internal class DotNetFrameworkEndOfLifeMonitor(IOptions<MonitorSettings> options, ILogger<DotNetFrameworkEndOfLifeMonitor> logger) : EndOfLifeFrameworkMonitor("dotnetfx", logger, options.Value)
{
    private static readonly string[] StandardVersions = ["1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "2.0", "2.1"];

    protected override Task<IReadOnlyCollection<Framework>> ParseAsync(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements, CancellationToken cancellationToken)
     => Task.FromResult(elements.Select(ToFramework).Union(DotnetStandard).ToArray() as IReadOnlyCollection<Framework>);

    private IEnumerable<Framework> DotnetStandard => StandardVersions.Select(ToStandardFramework);

    private Framework ToFramework(EndOfLifeHttpResponseElement source)
     => new()
     {
         Channel = source.Cycle.Replace(" SP1", string.Empty),
         EndOfLife = source.EndOfLife,
         Name = DotNetAssembly.FrameworkNameLegacy,
         ReleaseDate = source.ReleaseDate,
         Status = source.EndOfLife < DateTime.UtcNow ? FrameworkStatus.EndOfLife : FrameworkStatus.LongTermSupport,
         Version = source.Cycle.Replace(" SP1", string.Empty)
     };

    private Framework ToStandardFramework(string source)
    => new()
    {
        Channel = source,
        EndOfLife = new DateTime(2020, 11, 10, 0, 0, 0, DateTimeKind.Utc),
        Status = FrameworkStatus.EndOfLife,
        Version = source,
        Name = DotNetAssembly.FrameworkNameStandard,
    };
}
