using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Application.Model;
using System;
using System.Web;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationDbModel(ApplicationType type) : Application, IMongoEntity
{
    public ApplicationDbModel(Application application) : this(application.Type)
    {
        Name = application.Name;
        Path = application.Path;
        RepositoryLink = HttpUtility.UrlEncode(application.RepositoryLink);
        RepositoryType = application.RepositoryType;
        IsSoonDecommissioned = application.IsSoonDecommissioned;
        IsDeadLink = application.IsDeadLink;
        Branchs = [.. application.Branchs.Select(b => new ApplicationBranchDbModel(b))];
    }

    public ObjectId? Id { get; set; }

    public string AppType { get; set; } = type.ToString();

    public override ApplicationType Type => type;

    public Application ToDomain()
    {
        var appType = Enum.Parse<ApplicationType>(AppType);
        var applicationType = appType switch
        {
            ApplicationType.Angular => typeof(AngularApplication),
            ApplicationType.DotNet => typeof(DotNetApplication),
            ApplicationType.Php => typeof(PhpApplication),
            _ => throw new ArgumentOutOfRangeException(nameof(Type))
        };

        Application application = (Application)Activator.CreateInstance(applicationType)!;
        application.IsDeadLink = IsDeadLink;
        application.IsSoonDecommissioned = IsSoonDecommissioned;
        application.Name = Name;
        application.Path = Path;
        application.RepositoryLink = HttpUtility.UrlDecode(RepositoryLink);
        application.RepositoryType = RepositoryType;
        application.Branchs = [.. Branchs.OfType<ApplicationBranchDbModel>().Select(b => b.ToDomain(appType))];

        return application;
    }
}
