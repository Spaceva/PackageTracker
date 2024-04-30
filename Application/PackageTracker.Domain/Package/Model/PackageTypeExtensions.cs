namespace PackageTracker.Domain.Package.Model;

public static class PackageTypeExtensions
{
    public static PackageType ToPackageType(this Type type)
    {
        if (type.IsAssignableTo(typeof(Package)))
        {
            return type.FromPackageType();
        }

        throw new ArgumentOutOfRangeException(nameof(type));
    }

    public static Type ToPackageType(this PackageType packageType)
    {
        return packageType switch
        {
            PackageType.Npm => typeof(NpmPackage),
            PackageType.Nuget => typeof(NugetPackage),
            PackageType.Packagist => typeof(PackagistPackage),
            PackageType.Java => typeof(JavaPackage),
            _ => throw new ArgumentOutOfRangeException(nameof(packageType))
        };
    }

    private static PackageType FromPackageType(this Type type)
    {
        return type.Name switch
        {
            nameof(NpmPackage) => PackageType.Npm,
            nameof(NugetPackage) => PackageType.Nuget,
            nameof(PackagistPackage) => PackageType.Packagist,
            nameof(JavaPackage) => PackageType.Java,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }
}
