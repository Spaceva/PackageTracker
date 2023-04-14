using System.Text.RegularExpressions;

namespace PackageTracker.Domain.Packages;

public static partial class Constants
{
    public static class RegularExpressions
    {
        public static readonly Regex AnyVersionNumber = new("^(?<Major>\\d+)\\.(?<Minor>\\d+)\\.(?<Patch>\\d+-?[\\S]*)$");
        public static readonly Regex ReleaseVersionNumber = new("^(?<Major>\\d+)\\.(?<Minor>\\d+)\\.(?<Patch>\\d+)$");
    }
}
