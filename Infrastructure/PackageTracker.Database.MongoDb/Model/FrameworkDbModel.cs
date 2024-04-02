using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Framework.Model;

namespace PackageTracker.Database.MongoDb.Model;
internal class FrameworkDbModel : Framework, IMongoEntity
{
    public FrameworkDbModel(Framework framework)
    {
        Name = framework.Name;
        Version = framework.Version;
        Channel = framework.Channel;
        CodeName = framework.CodeName;
        Status = framework.Status;
        ReleaseDate = framework.ReleaseDate;
        EndOfLife = framework.EndOfLife;
    }
    
    public ObjectId? Id { get; set;  }
}
