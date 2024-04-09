namespace PackageTracker.Domain.Framework;

using System.Threading.Tasks;
using PackageTracker.Domain.Framework.Model;

public interface IFrameworkRepository
{
    Task<bool> ExistsAsync(string name, string version, CancellationToken cancellationToken = default);

    Task<Framework> GetByVersionAsync(string name, string version, CancellationToken cancellationToken = default);

    Task<Framework?> TryGetByVersionAsync(string name, string version, CancellationToken cancellationToken = default);

    Task DeleteByVersionAsync(string name, string version, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Framework>> SearchAsync(FrameworkSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default);

    Task SaveAsync(Framework framework, CancellationToken cancellationToken = default);
}
