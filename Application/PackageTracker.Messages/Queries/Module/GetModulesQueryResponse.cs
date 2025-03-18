using PackageTracker.Domain.Modules;

namespace PackageTracker.Messages.Queries;

public class GetModulesQueryResponse
{
    public IReadOnlyCollection<Module> Modules { get; init; } = [];
}