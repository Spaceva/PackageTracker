namespace PackageTracker.Messages.Queries;

public class GetAllPackagesQueryResponse
{
    public IReadOnlyCollection<PackageDto> Packages { get; init; } = Array.Empty<PackageDto>();

    public class PackageDto
    {
        public string Name { get; init; } = default!;

        public string LatestVersion { get; init; } = default!;

        public string? LatestReleaseVersion { get; init; }

        public string Type { get; init; } = default!;

        public string? Link { get; init; }
    }
}
