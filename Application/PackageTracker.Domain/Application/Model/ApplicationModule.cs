namespace PackageTracker.Domain.Application.Model;

using PackageTracker.Domain.Framework.Model;

public abstract class ApplicationModule
{
    public string Name { get; set; } = default!;

    public ICollection<ApplicationPackage> Packages { get; set; } = [];

    public Framework? Framework { get; set; }

    public string FrameworkVersion { get; set; } = default!;
}
