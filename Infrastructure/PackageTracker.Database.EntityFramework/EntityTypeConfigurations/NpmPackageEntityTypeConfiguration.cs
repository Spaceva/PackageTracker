using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;

internal class NpmPackageEntityTypeConfiguration : IEntityTypeConfiguration<NpmPackage>
{
    public void Configure(EntityTypeBuilder<NpmPackage> builder)
    {
        builder.HasBaseType<Package>();
    }
}
