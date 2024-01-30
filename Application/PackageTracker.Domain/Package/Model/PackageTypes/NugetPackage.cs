namespace PackageTracker.Domain.Package.Model;

public class NugetPackage : Package
{
    public override PackageType Type => PackageType.Nuget;
}
