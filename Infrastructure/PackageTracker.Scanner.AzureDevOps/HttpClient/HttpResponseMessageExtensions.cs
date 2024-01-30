namespace PackageTracker.Scanner.AzureDevOps;

internal static class HttpResponseMessageExtensions
{
    public static void EnsureJSONSuccess(this HttpResponseMessage httpResponseMessage)
    {
        httpResponseMessage.EnsureSuccessStatusCode();
        if (httpResponseMessage.Content.Headers?.ContentType?.MediaType != "application/json")
        {
            throw new HttpRequestException("Expected JSON response.");
        }
    }
}