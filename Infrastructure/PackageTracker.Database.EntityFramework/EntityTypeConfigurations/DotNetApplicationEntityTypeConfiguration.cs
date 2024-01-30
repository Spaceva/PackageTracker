using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class DotNetApplicationEntityTypeConfiguration : IEntityTypeConfiguration<DotNetApplication>
{
    public void Configure(EntityTypeBuilder<DotNetApplication> builder)
    {
        builder.HasBaseType<Application>();
    }
}
