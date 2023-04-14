using PackageTracker.Domain.Extensions;

namespace PackageTracker.Domain.Packages.Model;

public abstract class Package
{
    public string Name { get; set; } = default!;

    public ICollection<PackageVersion> Versions { get; set; } = default!;

    public abstract PackageType Type { get; }

    public string? LatestReleaseVersion
     => Versions.Where(v => Constants.RegularExpressions.ReleaseVersionNumber.IsMatch(v.Label))
                        .Select(v => Constants.RegularExpressions.ReleaseVersionNumber.MatchTo<VersionLabel>(v.Label))
                        .OrderByDescending(v => v.OrderingLabel)
                        .Select(v => v.Label)
                        .FirstOrDefault();

    public string LatestVersion
    => Versions.Where(v => Constants.RegularExpressions.AnyVersionNumber.IsMatch(v.Label))
                       .Select(v => Constants.RegularExpressions.AnyVersionNumber.MatchTo<VersionLabel>(v.Label))
                       .OrderByDescending(v => v.OrderingLabel)
                       .Select(v => v.Label)
                       .First();

    public IReadOnlyCollection<string> VersionLabelsDescending
     => Versions.Select(v => Constants.RegularExpressions.AnyVersionNumber.MatchTo<VersionLabel>(v.Label))
                        .OrderByDescending(v => v.OrderingLabel)
                        .Select(v => v.Label)
                        .ToArray();

    public string Link
     => Type switch
     {
         PackageType.Nuget => $"{Constants.RegistryUrls.NUGET_PACKAGE}/{Name}",
         PackageType.Npm => $"{Constants.RegistryUrls.NPM_PACKAGE}/{Name}",
         _ => "",
     };

    private class VersionLabel
    {
        public string Major { get; set; } = default!;
        public string Minor { get; set; } = default!;
        public string Patch { get; set; } = default!;
        public string Label => $"{Major}.{Minor}.{Patch}";
        public int OrderingMajor => int.Parse(Major);
        public int OrderingMinor => int.Parse(Minor);
        public string OrderingPatch
        {
            get
            {
                if (int.TryParse(Patch, out int patchInt))
                {
                    return $"{patchInt:0000000}";
                }

                return Patch;
            }
        }

        public string OrderingLabel => $"{OrderingMajor:0000}.{OrderingMinor:0000}.{OrderingPatch}";
    }
}
