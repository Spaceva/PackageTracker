using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class PhpApplicationEntityTypeConfiguration : IEntityTypeConfiguration<PhpApplication>
{
    public void Configure(EntityTypeBuilder<PhpApplication> builder)
    {
        builder.HasBaseType<Application>();
    }
}
