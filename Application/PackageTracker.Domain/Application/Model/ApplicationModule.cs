namespace PackageTracker.Domain.Application.Model;

using PackageTracker.Domain.Framework.Model;

public abstract class ApplicationModule
{
    public string Name { get; set; } = default!;

    public ICollection<ApplicationPackage> Packages { get; set; } = new List<ApplicationPackage>();

    public Framework? Framework { get; set; }
}
