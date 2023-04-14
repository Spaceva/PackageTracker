namespace PackageTracker.Domain.Packages.Model;

public class NugetPackage : Package
{
    public override PackageType Type => PackageType.Nuget;
}
