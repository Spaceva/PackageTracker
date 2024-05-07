namespace PackageTracker.Domain.Package.Model;

public class JavaPackage : Package
{
    public override PackageType Type => PackageType.Java;
}
