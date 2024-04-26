using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;

internal class JavaPackageEntityTypeConfiguration : IEntityTypeConfiguration<JavaPackage>
{
    public void Configure(EntityTypeBuilder<JavaPackage> builder)
    {
        builder.HasBaseType<Package>();
    }
}
