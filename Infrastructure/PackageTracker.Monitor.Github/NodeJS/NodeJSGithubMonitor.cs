using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Monitor.GitHub.NodeJS.Model;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PackageTracker.Monitor.GitHub.NodeJS;
internal class NodeJSGitHubMonitor(IOptions<MonitorSettings> monitoredFrameworkSettings, ILogger<NodeJSGitHubMonitor> logger) : GitHubFrameworkMonitor(monitoredFrameworkSettings.Value.Frameworks.SingleOrDefault(n => n.MonitorName.Equals(nameof(NodeJSGitHubMonitor))) ?? throw new ArgumentNullException(nameof(monitoredFrameworkSettings)),
    logger,
    monitoredFrameworkSettings.Value)
{
    protected override Task<IReadOnlyCollection<Framework>> ParseGitHubFileContentAsync(string decodedContent, CancellationToken cancellationToken)
    {
        var jsonObject = JsonNode.Parse(decodedContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true })?.AsObject() ?? throw new JsonException("Parsing failed.");
        var deserializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var versions = jsonObject.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Deserialize<VersionDetail>(deserializationOptions) ?? throw new JsonException("Version detail is badly formatted."));
        var frameworks = new List<Framework>();
        foreach (var version in versions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var channel = version.Key;
            var versionDetail = version.Value;
            var frameworkChannel = new Framework
            {
                Channel = channel.Replace("v", string.Empty),
                CodeName = versionDetail.Codename,
                EndOfLife = versionDetail.End,
                Name = Constants.FrameworkName,
                ReleaseDate = versionDetail.Start,
                Version = channel.Replace("v", string.Empty)
            };
            var activeStatus = versionDetail.Lts < DateTime.UtcNow ? FrameworkStatus.LongTermSupport : FrameworkStatus.Active;
            var nonPreviewStatus = (versionDetail.End is null || versionDetail.End > DateTime.UtcNow) ? activeStatus : FrameworkStatus.EndOfLife;
            frameworkChannel.Status = versionDetail.Start > DateTime.UtcNow ? FrameworkStatus.Preview : nonPreviewStatus;
            frameworks.Add(frameworkChannel);
        }

        return Task.FromResult(frameworks as IReadOnlyCollection<Framework>);
    }
}
