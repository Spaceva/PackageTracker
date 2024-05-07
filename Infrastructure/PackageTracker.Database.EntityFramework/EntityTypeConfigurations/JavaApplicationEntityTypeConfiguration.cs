using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Database.EntityFramework;

internal class JavaApplicationEntityTypeConfiguration : IEntityTypeConfiguration<JavaApplication>
{
    public void Configure(EntityTypeBuilder<JavaApplication> builder)
    {
        builder.HasBaseType<Application>();
    }
}
