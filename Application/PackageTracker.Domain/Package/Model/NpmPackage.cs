namespace PackageTracker.Domain.Packages.Model;

public class NpmPackage : Package
{
    public override PackageType Type => PackageType.Npm;
}
