using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MemoryCache.Cloners;
internal class FrameworkCloner : BaseCloner<Framework?>
{
    public override Framework? Clone(Framework? duplicatedObject)
    {
        if(duplicatedObject is null)
        {
            return null;
        }

        return new()
        {
            Name = duplicatedObject.Name,
            Version = duplicatedObject.Version,
            Channel = duplicatedObject.Channel,
            CodeName = duplicatedObject.CodeName,
            Status = duplicatedObject.Status,
            ReleaseDate = duplicatedObject.ReleaseDate,
            EndOfLife = duplicatedObject.EndOfLife,
        };
    }
}
