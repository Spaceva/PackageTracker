namespace PackageTracker.Scanner;

public static partial class Constants
{
    public static class Git
    {
        public static readonly IEnumerable<string> ValidBranches = new[] { "main", "master", "develop" };
    }
}
