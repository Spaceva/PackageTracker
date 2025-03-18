using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Database.MongoDb.Model;

internal class ModuleDbModel(Module module) : IMongoEntity
{
    public ObjectId? Id { get; set; }

    public string Name { get; set; } = module.Name;

    public bool IsEnabled { get; set; } = module.IsEnabled;

    public Module ToModule() => new() { Name = Name, IsEnabled = IsEnabled };
}
