using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;
internal class ApplicationBranchModel
{
    public string Name { get; set; } = default!;
    
    public string? Link { get; set; }

    public ApplicationType ApplicationType { get; set; }

    public ICollection<ApplicationModuleModel> Modules { get; set; } = [];

    public DateTime? LastCommit { get; set; }
}
