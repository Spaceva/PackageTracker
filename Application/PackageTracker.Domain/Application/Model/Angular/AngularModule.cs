namespace PackageTracker.Domain.Application.Model;

public class AngularModule : ApplicationModule
{
    public const string FrameworkName = "Angular";

    public string AngularVersion { get; set; } = default!;
}
