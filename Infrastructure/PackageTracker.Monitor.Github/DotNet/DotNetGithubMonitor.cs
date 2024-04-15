using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Framework.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace PackageTracker.Monitor.GitHub.DotNet;
internal class DotNetGitHubMonitor(IOptions<MonitorSettings> monitoredFrameworkSettings, ILogger<DotNetGitHubMonitor> logger) : GitHubFrameworkMonitor(
    monitoredFrameworkSettings.Value.Frameworks.SingleOrDefault(n => n.MonitorName.Equals(nameof(DotNetGitHubMonitor))) ?? throw new ArgumentNullException(nameof(monitoredFrameworkSettings)),
    logger,
    monitoredFrameworkSettings.Value)
{
    protected override async Task<IReadOnlyCollection<Framework>> ParseGitHubFileContentAsync(string decodedContent, CancellationToken cancellationToken)
    {
        var mainIndexFile = JsonSerializer.Deserialize<MainIndexFile>(decodedContent) ?? throw new ArgumentException(null, nameof(decodedContent));

        var frameworks = new List<Framework>();
        foreach (var releaseIndex in mainIndexFile.ReleasesIndex)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var baseFrameworkChannel = new Framework
            {
                Channel = releaseIndex.ChannelVersion,
                CodeName = releaseIndex.Product,
                EndOfLife = releaseIndex.EolDate,
                Name = DotNetAssembly.FrameworkName
            };
            var activeStatus = releaseIndex.ReleaseType switch
            {
                Constants.ReleaseTypes.STS => FrameworkStatus.Active,
                Constants.ReleaseTypes.LTS => FrameworkStatus.LongTermSupport,
                _ => throw new ArgumentException(nameof(releaseIndex.ReleaseType))
            };
            baseFrameworkChannel.Status = releaseIndex.SupportPhase switch
            {
                Constants.SupportPhases.Active => activeStatus,
                Constants.SupportPhases.Maintenance => activeStatus,
                Constants.SupportPhases.EOL => FrameworkStatus.EndOfLife,
                _ => FrameworkStatus.Preview,
            };

            var channelContent = await HttpClient.GetAsync(releaseIndex.ReleasesJson, cancellationToken);
            channelContent.EnsureSuccessStatusCode();
            var channelDetail = await channelContent.Content.ReadFromJsonAsync<ChannelDetail>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken) ?? throw new ArgumentException(nameof(releaseIndex.ReleasesJson));
            frameworks.AddRange(channelDetail.Releases.Select(r => FrameworkFrom(baseFrameworkChannel, r.ReleaseVersion, r.ReleaseDate)));
        }

        return frameworks;
    }

    private static Framework FrameworkFrom(Framework baseFrameworkChannel, string releaseVersion, DateTime releaseDate)
    {
        var status = baseFrameworkChannel.Status;
        if ((status == FrameworkStatus.Active || status == FrameworkStatus.LongTermSupport)
            && !Domain.Package.Constants.RegularExpressions.ReleaseVersionNumber.IsMatch(releaseVersion))
        {
            status = FrameworkStatus.EndOfLife;
        }
        return new()
        {
            Version = releaseVersion,
            ReleaseDate = releaseDate,
            Channel = baseFrameworkChannel.Channel,
            CodeName = baseFrameworkChannel.CodeName,
            EndOfLife = baseFrameworkChannel.EndOfLife,
            Name = baseFrameworkChannel.Name,
            Status = status,
        };
    }
}
