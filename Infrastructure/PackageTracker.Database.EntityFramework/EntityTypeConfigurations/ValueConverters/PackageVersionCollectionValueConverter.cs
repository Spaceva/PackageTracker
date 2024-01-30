using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageTracker.Domain.Package.Model;

namespace PackageTracker.Database.EntityFramework;
internal class PackageVersionCollectionValueConverter : ValueConverter<ICollection<PackageVersion>, string>
{
    public PackageVersionCollectionValueConverter()
        : base(x => string.Join(", ", x.Select(v => v.ToString())), x => x.Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(v => new PackageVersion(v)).ToList())
    {

    }
}
