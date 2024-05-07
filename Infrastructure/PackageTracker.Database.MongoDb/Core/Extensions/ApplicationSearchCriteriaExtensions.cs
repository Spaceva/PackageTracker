using MongoDB.Driver;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Domain.Application;

namespace PackageTracker.Database.MongoDb.Core;
internal static class ApplicationSearchCriteriaExtensions
{
    public static FilterDefinition<ApplicationDbModel> ToFilterDefinition(this ApplicationSearchCriteria? searchCriteria)
    {
        var filterBuilder = Builders<ApplicationDbModel>.Filter;
        FilterDefinition<ApplicationDbModel> searchFilterDefinition = FilterDefinition<ApplicationDbModel>.Empty;
        if (searchCriteria is null)
        {
            return searchFilterDefinition;
        }

        if (searchCriteria.ApplicationTypes?.Count > 0)
        {
            searchFilterDefinition &= filterBuilder.In(nameof(ApplicationDbModel.AppType).ToCamelCase(), searchCriteria.ApplicationTypes.Select(a => a.ToString()));
        }

        if (searchCriteria.RepositoryTypes?.Count > 0)
        {
            searchFilterDefinition &= filterBuilder.In(nameof(ApplicationDbModel.RepositoryType).ToCamelCase(), searchCriteria.RepositoryTypes.Select(a => a.ToString()));
        }

        if (searchCriteria.ApplicationName?.Length > 0)
        {
            searchFilterDefinition &= filterBuilder.Regex(a => a.Name, new MongoDB.Bson.BsonRegularExpression("/" + searchCriteria.ApplicationName + "/"));
        }

        if (!searchCriteria.ShowSoonDecommissioned)
        {
            searchFilterDefinition &= filterBuilder.Where(app => !app.IsSoonDecommissioned);
        }

        if (!searchCriteria.ShowDeadLink)
        {
            searchFilterDefinition &= filterBuilder.Where(app => !app.IsDeadLink);
        }

        if (searchCriteria.LastCommitAfter.HasValue)
        {
            searchFilterDefinition &= searchCriteria.LastCommitAfter.Value.ToFilterDefinitionAfter(filterBuilder, searchCriteria.ApplyCommitFilterOnAllBranchs);
        }

        if (searchCriteria.LastCommitBefore.HasValue)
        {
            searchFilterDefinition &= searchCriteria.LastCommitBefore.Value.ToFilterDefinitionBefore(filterBuilder, searchCriteria.ApplyCommitFilterOnAllBranchs);
        }

        return searchFilterDefinition;
    }

    private static FilterDefinition<ApplicationDbModel> ToFilterDefinitionAfter(this DateTime dateTime, FilterDefinitionBuilder<ApplicationDbModel> filterBuilder, bool applyCommitFilterOnAllBranchs)
    {
        if (applyCommitFilterOnAllBranchs)
        {
            return filterBuilder.Not(filterBuilder.ElemMatch(app => app.Branchs, Builders<ApplicationBranchDbModel>.Filter.Lt(b => b.LastCommit!, dateTime)));
        }
        else
        {
            return filterBuilder.ElemMatch(app => app.Branchs, Builders<ApplicationBranchDbModel>.Filter.Gte(b => b.LastCommit!, dateTime));
        }
    }

    private static FilterDefinition<ApplicationDbModel> ToFilterDefinitionBefore(this DateTime dateTime, FilterDefinitionBuilder<ApplicationDbModel> filterBuilder, bool applyCommitFilterOnAllBranchs)
    {
        if (applyCommitFilterOnAllBranchs)
        {
            return filterBuilder.Not(filterBuilder.ElemMatch(app => app.Branchs, Builders<ApplicationBranchDbModel>.Filter.Gt(b => b.LastCommit!, dateTime)));
        }
        else
        {
            return filterBuilder.ElemMatch(app => app.Branchs, Builders<ApplicationBranchDbModel>.Filter.Lte(b => b.LastCommit!, dateTime));
        }
    }
}
