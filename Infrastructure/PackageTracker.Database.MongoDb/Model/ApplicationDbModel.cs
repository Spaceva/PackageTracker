using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Application.Model;
using System.Web;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationDbModel(ApplicationType type) : IMongoEntity
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

    public string Name { get; set; } = default!;

    public string Path { get; set; } = default!;

    public string RepositoryLink { get; set; } = default!;

    public ICollection<ApplicationBranchDbModel> Branchs { get; set; } = [];

    public ApplicationType Type => type;

    public RepositoryType RepositoryType { get; set; }

    public bool IsSoonDecommissioned { get; set; }

    public bool IsDeadLink { get; set; }

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
        application.Branchs = [.. Branchs.Select(b => b.ToDomain(appType))];

        return application;
    }
}
