namespace PackageTracker.Domain.Package.Model;

public class PackagistPackage : Package
{
    public override PackageType Type => PackageType.Packagist;
}
