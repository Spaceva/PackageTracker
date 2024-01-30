using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class AngularApplicationEntityTypeConfiguration : IEntityTypeConfiguration<AngularApplication>
{
    public void Configure(EntityTypeBuilder<AngularApplication> builder)
    {
        builder.HasBaseType<Application>();
    }
}
