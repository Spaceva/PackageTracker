using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Presentation.MVCApp.Models;

public class TrackPackageViewModel
{
    public required string PackageName { get; init; }
    public required PackageType PackageType { get; init; }
}