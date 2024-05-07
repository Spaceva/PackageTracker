namespace PackageTracker.Domain.Application.Model;

public static class ApplicationTypeExtensions
{
    public static ApplicationType ToApplicationType(this Type type)
    {
        if (type.IsAssignableTo(typeof(Application)))
        {
            return type.FromApplicationType();
        }

        if (type.IsAssignableTo(typeof(ApplicationBranch)))
        {
            return type.FromApplicationBranchType();
        }

        if (type.IsAssignableTo(typeof(ApplicationModule)))
        {
            return type.FromApplicationModuleType();
        }

        throw new ArgumentOutOfRangeException(nameof(type));
    }

    public static Type ToApplicationType(this ApplicationType applicationType)
    {
        return applicationType switch
        {
            ApplicationType.Angular => typeof(AngularApplication),
            ApplicationType.Php => typeof(PhpApplication),
            ApplicationType.Java => typeof(JavaApplication),
            ApplicationType.DotNet => typeof(DotNetApplication),
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType))
        };
    }

    public static Type ToApplicationBranchType(this ApplicationType applicationType)
    {
        return applicationType switch
        {
            ApplicationType.Angular => typeof(AngularApplicationBranch),
            ApplicationType.Php => typeof(PhpApplicationBranch),
            ApplicationType.Java => typeof(JavaApplicationBranch),
            ApplicationType.DotNet => typeof(DotNetApplicationBranch),
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType))
        };
    }

    public static Type ToApplicationModuleType(this ApplicationType applicationType)
    {
        return applicationType switch
        {
            ApplicationType.Angular => typeof(AngularModule),
            ApplicationType.Php => typeof(PhpModule),
            ApplicationType.Java => typeof(JavaModule),
            ApplicationType.DotNet => typeof(DotNetAssembly),
            _ => throw new ArgumentOutOfRangeException(nameof(applicationType))
        };
    }

    private static ApplicationType FromApplicationType(this Type type)
    {
        return type.Name switch
        {
            nameof(AngularApplication) => ApplicationType.Angular,
            nameof(PhpApplication) => ApplicationType.Php,
            nameof(JavaApplication) => ApplicationType.Java,
            nameof(DotNetApplication) => ApplicationType.DotNet,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    private static ApplicationType FromApplicationBranchType(this Type type)
    {
        return type.Name switch
        {
            nameof(AngularApplicationBranch) => ApplicationType.Angular,
            nameof(PhpApplicationBranch) => ApplicationType.Php,
            nameof(JavaApplicationBranch) => ApplicationType.Java,
            nameof(DotNetApplicationBranch) => ApplicationType.DotNet,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    private static ApplicationType FromApplicationModuleType(this Type type)
    {
        return type.Name switch
        {
            nameof(AngularModule) => ApplicationType.Angular,
            nameof(PhpModule) => ApplicationType.Php,
            nameof(JavaModule) => ApplicationType.Java,
            nameof(DotNetAssembly) => ApplicationType.DotNet,
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }
}
