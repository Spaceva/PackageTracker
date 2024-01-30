using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class ApplicationEntityTypeConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.HasKey(x => new { x.Name, x.RepositoryLink });

        builder.HasIndex(x => x.Name);

        builder.Ignore(x => x.Type);

        builder.Property(x => x.RepositoryType).HasConversion<string>();

        builder.Property(x => x.Branchs)
                .HasConversion(new ApplicationBranchCollectionValueConverter(), new ApplicationBranchCollectionValueComparer());

        builder.Property(x => x.IsSoonDecommissioned).HasDefaultValue(false);

        builder.Property(x => x.IsDeadLink).HasDefaultValue(false);
    }
}
