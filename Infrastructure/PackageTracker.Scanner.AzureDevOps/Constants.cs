using System.Text.Json;

namespace PackageTracker.Scanner.AzureDevOps;

internal static partial class Constants
{
    public static class Urls
    {
        public const string ApiUrl = "_apis/git/repositories";
        public const string RepositoriesList = ApiUrl + "/?api-version=7.0";
        public const string RepositoryBranchsList = ApiUrl + "/{0}/refs?api-version=7.0";
        public const string RootDetail = ApiUrl + "/{0}/items?path=%2F&api-version=7.0";
        public const string FilesList = ApiUrl + "/{0}/trees/{1}?recursive=true&versionDescriptor.version={2}&api-version=7.0";
        public const string FileDetail = ApiUrl + "/{0}/blobs/{1}?versionDescriptor.version={2}&$format=text&api-version=7.0";
        public const string LastCommit = ApiUrl + "/{0}/commits?searchCriteria.$top=1&searchCriteria.itemVersion.version={1}&searchCriteria.itemVersion.versionType=branch&api-version=7.0";
    }

    internal static class JsonSettings
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new() { IgnoreReadOnlyProperties = true, IgnoreReadOnlyFields = true, PropertyNameCaseInsensitive = true, WriteIndented = false };
    }
}