using Microsoft.EntityFrameworkCore;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Domain.Notifications.Model;
using PackageTracker.Domain.Package.Model;
using System.Reflection;

namespace PackageTracker.Infrastructure;
internal class PackageTrackerDbContext(DbContextOptions<PackageTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Package> Packages { get; set; }
    public DbSet<Application> Applications { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Framework> Frameworks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}