using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;

internal class PackagistPackageEntityTypeConfiguration : IEntityTypeConfiguration<PackagistPackage>
{
    public void Configure(EntityTypeBuilder<PackagistPackage> builder)
    {
        builder.HasBaseType<Package>();
    }
}
