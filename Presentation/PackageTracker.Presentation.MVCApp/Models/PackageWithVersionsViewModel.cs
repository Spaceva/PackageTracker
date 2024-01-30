namespace PackageTracker.Presentation.MVCApp.Models;

public class PackageWithVersionsViewModel : PackageViewModel
{
    public required IDictionary<string, string> Versions { get; init; }
}
