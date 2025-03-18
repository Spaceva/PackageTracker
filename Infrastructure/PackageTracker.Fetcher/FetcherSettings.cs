using PackageTracker.Infrastructure.Http;
using PackageTracker.Infrastructure.Modules;

namespace PackageTracker.Fetcher;

public class FetcherSettings : IModuleBackgroundSettings, IHttpProxy
{
    public TimeSpan TimeBetweenEachExecution { get; init; }

    public TrackedPackages? Packages { get; init; } = default!;

    public string? ProxyUrl { get; init; }

    public class TrackedPackages
    {
        public PublicTrackedPackages? Public { get; init; } = default!;

        public ICollection<PrivateTrackedPackages> Privates { get; init; } = default!;
        
        public class PublicTrackedPackages
        {
            public ICollection<string>? Npm { get; init; }

            public ICollection<string>? Nuget { get; init; }

            public ICollection<string>? Packagist { get; init; }

            public ICollection<string>? MavenCentral { get; init; }
        }

        public class PrivateTrackedPackages
        {
            public string FetcherName { get; init; } = default!;

            public string RepositoryLink { get; init; } = default!;

            public ICollection<string>? PackagesName { get; init; }
        }
    }
}
