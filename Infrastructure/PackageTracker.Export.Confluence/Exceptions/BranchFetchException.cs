namespace PackageTracker.Export.Confluence.Exceptions;

public class BranchFetchException : Exception
{
    public BranchFetchException() : base($"Failed to search branch with one of the following names : {string.Join(',', Constants.Git.ValidBranches)}") { }
}
