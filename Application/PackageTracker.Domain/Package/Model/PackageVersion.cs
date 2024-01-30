namespace PackageTracker.Domain.Package.Model;

public class PackageVersion
{
    public PackageVersion(string version)
    {
        if (!Constants.RegularExpressions.AnyVersionNumber.IsMatch(version))
        {
            throw new ArgumentException($"Expected version at format '{Constants.RegularExpressions.AnyVersionNumber}', got '{version}'.", nameof(version));
        }

        var separatedString = version.Split(".");
        Major = int.Parse(separatedString[0]);

        if (separatedString.Length < 2)
        {
            return;
        }

        Minor = int.Parse(separatedString.Skip(1).First());

        if (separatedString.Length < 3)
        {
            return;
        }

        Patch = string.Join(".", separatedString.Skip(2));
    }

    public int Major { get; set; } = 0;
    public int Minor { get; set; } = 0;
    public string Patch { get; set; } = "0";

    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    public string ToStringMajorMinor() => $"{Major}.{Minor}";

    public bool IsRelease => Constants.RegularExpressions.ReleaseVersionNumber.IsMatch(ToString());

    public bool IsPreRelease => !IsRelease;

    public override bool Equals(object? obj)
    {
        if (obj is string versionLabel)
        {
            return ToString().Equals(versionLabel, StringComparison.OrdinalIgnoreCase);
        }

        if (obj is PackageVersion packageVersion)
        {
            return ToString().Equals(packageVersion.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public override int GetHashCode()
     => ToString().GetHashCode();
}
