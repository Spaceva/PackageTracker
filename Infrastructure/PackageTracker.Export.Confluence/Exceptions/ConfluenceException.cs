namespace PackageTracker.Export.Confluence.Exceptions;

public class ConfluenceException(Exception? innerException) : Exception("An error occured when trying to reach Confluence. See inner exception for details.", innerException)
{
}
