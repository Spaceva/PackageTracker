namespace PackageTracker.Scanner.AzureDevOps.HttpClient.HttpResponses;
internal class GetLastCommitHttpResponse
{
    public int? Count { get; init; }

    public IReadOnlyCollection<Commit> Value { get; init; } = Array.Empty<Commit>();

    public DateTime? LastCommitDate => Value.FirstOrDefault()?.Committer?.Date;

    public class Commit
    {
        public string CommitId { get; init; } = default!;
        public Committer? Committer { get; init; }
        public string? Comment { get; init; }
        public bool? CommentTruncated { get; init; }
        public string? Url { get; init; }
        public string? RemoteUrl { get; init; }
    }

    public class Committer
    {
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public DateTime? Date { get; init; }
    }
}
