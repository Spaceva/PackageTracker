namespace PackageTracker.Scanner;

public static class Constants
{
    public const string ModuleName = "Scanner";

    public static class Git
    {
        public static readonly IEnumerable<string> ValidBranches = ["main", "master", "develop"];
    }
}
