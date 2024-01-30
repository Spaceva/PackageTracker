using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;

internal class PackageEntityTypeConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.HasKey(x => x.Name);

        builder.Ignore(x => x.LatestVersion);
        builder.Ignore(x => x.LatestReleaseVersion);
        builder.Ignore(x => x.Type);
        builder.Property(x => x.Versions).HasConversion(new PackageVersionCollectionValueConverter(), new PackageVersionCollectionComparer());
    }
}
