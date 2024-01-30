using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationModuleModel
{
    public string Name { get; set; } = default!;

    public ApplicationType ModuleType { get; set; }

    public string MainFrameworkVersion { get; set; } = default!;

    public ICollection<ApplicationPackageModel> Packages { get; set; } = new List<ApplicationPackageModel>();
}
