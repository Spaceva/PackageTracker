using System.Text.Json;

namespace PackageTracker.Scanner.AzureDevOps;

internal static partial class Constants
{
    public static class Urls
    {
        public const string ApiUrl = "_apis/git/repositories";
        public const string ApiVersion = "api-version=7.1";

        public const string RepositoriesList = $"{ApiUrl}/?{ApiVersion}";
        public const string RepositoryBranchsList = ApiUrl + "/{0}/refs?" + ApiVersion;
        public const string RootDetail = ApiUrl + "/{0}/items?path=%2F&" + ApiVersion;
        public const string FilesList = ApiUrl + "/{0}/trees/{1}?recursive=true&versionDescriptor.version={2}&" + ApiVersion;
        public const string FileDetail = ApiUrl + "/{0}/blobs/{1}?versionDescriptor.version={2}&$format=text&" + ApiVersion;
        public const string LastCommit = ApiUrl + "/{0}/commits?searchCriteria.$top=1&searchCriteria.itemVersion.version={1}&searchCriteria.itemVersion.versionType=branch&" + ApiVersion;
    }

    internal static class JsonSettings
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            IgnoreReadOnlyProperties = true,
            IgnoreReadOnlyFields = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }
}