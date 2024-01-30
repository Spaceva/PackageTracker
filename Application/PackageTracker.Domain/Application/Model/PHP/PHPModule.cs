namespace PackageTracker.Domain.Application.Model;

public class PhpModule : ApplicationModule
{
    public const string FrameworkName = "PHP";

    public string PhpVersion { get; set; } = default!;
}
