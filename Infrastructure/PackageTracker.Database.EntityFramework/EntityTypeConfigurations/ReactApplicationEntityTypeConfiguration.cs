using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class ReactApplicationEntityTypeConfiguration : IEntityTypeConfiguration<ReactApplication>
{
    public void Configure(EntityTypeBuilder<ReactApplication> builder)
    {
        builder.HasBaseType<Application>();
    }
}
