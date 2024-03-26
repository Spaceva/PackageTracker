using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using System.Web;

namespace PackageTracker.Database.MongoDb.Repositories;
internal class ApplicationsDbRepository(MongoDbContext dbContext, ILogger<ApplicationsDbRepository> logger) : BaseDbRepository<ApplicationDbModel>(dbContext, logger), IApplicationsRepository
{
    public async Task DeleteAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        await DeleteByQueryAsync(Filter.Eq(a => a.Name, name) & Filter.Eq(a => a.AppType, applicationType.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(repositoryLink)), cancellationToken);
    }

    public async Task<Application> GetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        return await TryGetAsync(name, applicationType, repositoryLink, cancellationToken) ?? throw new ApplicationNotFoundException();
    }

    public async Task SaveAsync(Application application, CancellationToken cancellationToken = default)
    {
        await UpdateAsync(Filter.Eq(a => a.Name, application.Name) & Filter.Eq(a => a.AppType, application.Type.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(application.RepositoryLink)), new ApplicationDbModel(application), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Application>> SearchAsync(ApplicationSearchCriteria searchCriteria, int? skip = null, int? take = null, CancellationToken cancellationToken = default)
    {
        var results = await FindAsync(SearchCriteria(searchCriteria), cancellationToken);

        return [.. results.Select(app => app.ToDomain())];
    }

    public async Task<Application?> TryGetAsync(string name, ApplicationType applicationType, string repositoryLink, CancellationToken cancellationToken = default)
    {
        return (await GetAsync(Filter.Eq(a => a.Name, name) & Filter.Eq(a => a.AppType, applicationType.ToString()) & Filter.Eq(a => a.RepositoryLink, HttpUtility.UrlEncode(repositoryLink)), cancellationToken))?.ToDomain();
    }

    private static FilterDefinition<ApplicationDbModel> SearchCriteria(ApplicationSearchCriteria? searchCriteria)
    {
        FilterDefinition<ApplicationDbModel> searchFilterDefinition = FilterDefinition<ApplicationDbModel>.Empty;
        if (searchCriteria is null)
        {
            return searchFilterDefinition;
        }

        if (searchCriteria.ApplicationTypes is not null && searchCriteria.ApplicationTypes.Count > 0)
        {
            searchFilterDefinition &= Filter.AnyIn(nameof(ApplicationDbModel.AppType).ToCamelCase(), searchCriteria.ApplicationTypes.Select(a => a.ToString()));
        }

        if (searchCriteria.RepositoryTypes is not null && searchCriteria.RepositoryTypes.Count > 0)
        {
            searchFilterDefinition &= Filter.AnyIn(nameof(ApplicationDbModel.RepositoryType).ToCamelCase(), searchCriteria.RepositoryTypes);
        }

        if (searchCriteria.ApplicationName is not null && searchCriteria.ApplicationName.Length > 0)
        {
            searchFilterDefinition &= Filter.Regex(a => a.Name, new MongoDB.Bson.BsonRegularExpression("/" + searchCriteria.ApplicationName + "/"));
        }

        if (!searchCriteria.ShowSoonDecommissioned)
        {
            searchFilterDefinition &= Filter.Where(app => !app.IsSoonDecommissioned);
        }

        if (!searchCriteria.ShowDeadLink)
        {
            searchFilterDefinition &= Filter.Where(app => !app.IsDeadLink);
        }

        if (searchCriteria.LastCommitAfter.HasValue)
        {
            if (searchCriteria.ApplyCommitFilterOnAllBranchs)
            {
                searchFilterDefinition &= Filter.Not(Filter.ElemMatch(app => app.Branchs, Builders<ApplicationBranch>.Filter.Lt(b => b.LastCommit!, searchCriteria.LastCommitAfter)));
            }
            else
            {
                searchFilterDefinition &= Filter.ElemMatch(app => app.Branchs, Builders<ApplicationBranch>.Filter.Gte(b => b.LastCommit!, searchCriteria.LastCommitAfter));
            }
        }

        if (searchCriteria.LastCommitBefore.HasValue)
        {
            if (searchCriteria.ApplyCommitFilterOnAllBranchs)
            {
                searchFilterDefinition &= Filter.Not(Filter.ElemMatch(app => app.Branchs, Builders<ApplicationBranch>.Filter.Gt(b => b.LastCommit!, searchCriteria.LastCommitBefore)));
            }
            else
            {
                searchFilterDefinition &= Filter.ElemMatch(app => app.Branchs, Builders<ApplicationBranch>.Filter.Lte(b => b.LastCommit!, searchCriteria.LastCommitBefore));
            }
        }

        return searchFilterDefinition;
    }
}
