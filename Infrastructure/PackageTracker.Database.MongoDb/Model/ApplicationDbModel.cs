using MongoDB.Bson;
using PackageTracker.Database.MongoDb.Model.Base;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using System.Web;

namespace PackageTracker.Database.MongoDb.Model;
internal class ApplicationDbModel(Application application) : IMongoEntity
{
    public ObjectId? Id { get; set; }

    public string AppType { get; set; } = application.Type.ToString();

    public string Name { get; set; } = application.Name;

    public string Path { get; set; } = application.Path;

    public string RepositoryLink { get; set; } = HttpUtility.UrlEncode(application.RepositoryLink);

    public ICollection<ApplicationBranchDbModel> Branchs { get; set; } = [.. application.Branchs.Select(b => new ApplicationBranchDbModel(b))];

    public ApplicationType Type => Enum.Parse<ApplicationType>(AppType);

    public RepositoryType RepositoryType { get; set; } = application.RepositoryType;

    public bool IsSoonDecommissioned { get; set; } = application.IsSoonDecommissioned;

    public bool IsDeadLink { get; set; } = application.IsDeadLink;

    public Application ToDomain()
    {
        var appType = Enum.Parse<ApplicationType>(AppType);
        var applicationType = appType switch
        {
            ApplicationType.Angular => typeof(AngularApplication),
            ApplicationType.DotNet => typeof(DotNetApplication),
            ApplicationType.Php => typeof(PhpApplication),
            _ => throw new UnknownApplicationTypeException()
        };

        Application domainApplication = (Application)Activator.CreateInstance(applicationType)!;
        domainApplication.IsDeadLink = IsDeadLink;
        domainApplication.IsSoonDecommissioned = IsSoonDecommissioned;
        domainApplication.Name = Name;
        domainApplication.Path = Path;
        domainApplication.RepositoryLink = HttpUtility.UrlDecode(RepositoryLink);
        domainApplication.RepositoryType = RepositoryType;
        domainApplication.Branchs = [.. Branchs.Select(b => b.ToDomain(appType))];

        return domainApplication;
    }
}
