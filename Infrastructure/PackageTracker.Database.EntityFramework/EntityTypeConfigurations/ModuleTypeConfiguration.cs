using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Database.EntityFramework;

internal class ModuleTypeConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.HasKey(m => m.Name);
    }
}
