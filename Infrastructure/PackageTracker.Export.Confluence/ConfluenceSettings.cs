using PackageTracker.Infrastructure.Http;
using PackageTracker.Infrastructure.Modules;

namespace PackageTracker.Export.Confluence;

public class ConfluenceSettings : IModuleBackgroundSettings, IHttpProxy
{
    public string Username { get; init; } = default!;

    public string AccessToken { get; init; } = default!;

    public string Domain { get; init; } = default!;

    public IReadOnlyCollection<ConfluencePage> Pages { get; init; } = [];

    public TimeSpan TimeBetweenEachExecution { get; init; } = default!;

    public IReadOnlyCollection<CodeSourceCredentials> Credentials { get; init; } = [];

    public string? ProxyUrl { get; init; }

    public class ConfluencePage
    {
        public string Name { get; init; } = default!;

        public long Id { get; init; } = default!;
    }

    public class CodeSourceCredentials
    {
        public string Name { get; init; } = default!;
        public string Domain { get; init; } = default!;
        public string AccessToken { get; init; } = default!;
    }
}