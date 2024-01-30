namespace PackageTracker.Domain.Package.Model;

public class NpmPackage : Package
{
    public override PackageType Type => PackageType.Npm;
}
