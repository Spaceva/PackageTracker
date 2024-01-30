using System.Text.RegularExpressions;

namespace PackageTracker.Domain.Package;

public static partial class Constants
{
    public static partial class RegularExpressions
    {
        public static readonly Regex AnyVersionNumber = AnyVersionNumberRegex();
        public static readonly Regex ReleaseVersionNumber = ReleaseVersionNumberRegex();

        [GeneratedRegex("^(?<Major>\\d+)\\.(?<Minor>\\d+)\\.(?<Patch>\\d*[-*.]?[\\S]*)$")]
        private static partial Regex AnyVersionNumberRegex();
        
        [GeneratedRegex("^(?<Major>\\d+)\\.(?<Minor>\\d+)\\.(?<Patch>\\d+)$")]
        private static partial Regex ReleaseVersionNumberRegex();
    }
}
