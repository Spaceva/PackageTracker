using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;

internal class NugetPackageEntityTypeConfiguration : IEntityTypeConfiguration<NugetPackage>
{
    public void Configure(EntityTypeBuilder<NugetPackage> builder)
    {
        builder.HasBaseType<Package>();
    }
}
