using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Monitor.EndOfLife;
internal class PhpEndOfLifeMonitor(IOptions<MonitorSettings> options, ILogger<PhpEndOfLifeMonitor> logger) : EndOfLifeFrameworkMonitor("php", logger, options.Value)
{
    protected override Task<IReadOnlyCollection<Framework>> ParseAsync(IReadOnlyCollection<EndOfLifeHttpResponseElement> elements, CancellationToken cancellationToken)
     => Task.FromResult(elements.Select(ToFramework).ToArray() as IReadOnlyCollection<Framework>);

    private Framework ToFramework(EndOfLifeHttpResponseElement source)
     => new()
     {
         Channel = source.Cycle,
         EndOfLife = source.EndOfLife,
         Name = PhpModule.FrameworkName,
         ReleaseDate = source.ReleaseDate,
         Status = source.EndOfLife < DateTime.UtcNow ? FrameworkStatus.EndOfLife : FrameworkStatus.LongTermSupport,
         Version = source.Cycle
     };
}
