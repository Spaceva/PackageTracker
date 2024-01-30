using System.Net;

namespace PackageTracker.Infrastructure.Http;

public static class HttpClientFactory
{
    public static HttpClient Build(IHttpProxy? httpProxy = null, string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(httpProxy?.ProxyUrl))
        {
            return BuildWithoutProxy(baseUrl);
        }

        return BuildWithProxy(httpProxy, baseUrl);
    }

    private static HttpClientHandler HttpClientHandler(IHttpProxy httpProxy)
    {
        var proxy = new WebProxy
        {
            Address = new Uri(httpProxy.ProxyUrl!),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,
        };

        return new HttpClientHandler
        {
            Proxy = proxy,
            ServerCertificateCustomValidationCallback = System.Net.Http.HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    }

    private static HttpClient BuildWithProxy(IHttpProxy httpProxy, string? baseUrl)
    {
        var httpClient = BuildHandler(HttpClientHandler(httpProxy));
        if (baseUrl is not null)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
        }

        return httpClient;
    }

    private static HttpClient BuildWithoutProxy(string? baseUrl)
    {
        if (baseUrl is null)
        {
            return BuildDefault();
        }

        return new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    private static HttpClient BuildHandler(HttpClientHandler httpClientHandler)
     => new(httpClientHandler, true);

    private static HttpClient BuildDefault()
     => new();
}
