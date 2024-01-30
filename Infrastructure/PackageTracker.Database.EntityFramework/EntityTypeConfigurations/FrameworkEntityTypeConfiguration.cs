using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.EntityFramework;

internal class FrameworkEntityTypeConfiguration : IEntityTypeConfiguration<Framework>
{
    public void Configure(EntityTypeBuilder<Framework> builder)
    {
        builder.HasKey(x => new { x.Name, x.Version});

        builder.HasIndex(x => x.Name);

        builder.HasIndex(x => x.Channel);

        builder.Property(x => x.Status).HasConversion<string>();
    }
}
