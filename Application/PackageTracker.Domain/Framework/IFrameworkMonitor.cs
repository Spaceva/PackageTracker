namespace PackageTracker.Domain.Framework;

using PackageTracker.Domain.Framework.Model;
using System.Threading.Tasks;

public interface IFrameworkMonitor : IDisposable
{
    Task<IReadOnlyCollection<Framework>> MonitorAsync(CancellationToken cancellationToken);
}
