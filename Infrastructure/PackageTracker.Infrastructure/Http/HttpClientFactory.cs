using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PackageTracker.Infrastructure.Http;

public static class HttpClientFactory
{
    public static HttpClient Build(IHttpProxy? httpProxy = null, string? baseUrl = null, bool acceptPartialCertificates = false)
    {
        if (string.IsNullOrWhiteSpace(httpProxy?.ProxyUrl))
        {
            return BuildWithoutProxy(baseUrl, acceptPartialCertificates);
        }

        return BuildWithProxy(httpProxy, baseUrl, acceptPartialCertificates);
    }

    private static HttpClient BuildWithProxy(IHttpProxy httpProxy, string? baseUrl, bool acceptPartialCertificates)
    {
        HttpClient httpClient = new(HttpClientHandler(httpProxy, baseUrl, acceptPartialCertificates), true);
        if (baseUrl is not null)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
        }

        return httpClient;
    }

    private static HttpClient BuildWithoutProxy(string? baseUrl, bool acceptPartialCertificates)
    {
        HttpClient httpClient = new(HttpClientHandler(baseUrl, acceptPartialCertificates), true);
        if (baseUrl is not null)
        {
            httpClient.BaseAddress = new Uri(baseUrl);
        }

        return httpClient;
    }

    private static HttpClientHandler HttpClientHandler(string? baseUrl, bool acceptPartialCertificates)
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = baseUrl is not null && acceptPartialCertificates is true ? ServerCertificateValidationPartial(baseUrl) : ServerCertificateValidation
        };
    }

    private static Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> ServerCertificateValidationPartial(string baseUrl)
     => (HttpRequestMessage message, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors errors) => ServerCertificateValidation(message, certificate, chain, errors) || IsAllowedOrigin(message, baseUrl);

    private static HttpClientHandler HttpClientHandler(IHttpProxy httpProxy, string? baseUrl, bool acceptPartialCertificates)
    {
        var proxy = new WebProxy
        {
            Address = new Uri(httpProxy.ProxyUrl!),
            BypassProxyOnLocal = false,
        };

        return new HttpClientHandler
        {
            Proxy = proxy,
            ServerCertificateCustomValidationCallback = baseUrl is not null && acceptPartialCertificates is true ? ServerCertificateValidationPartial(baseUrl) : ServerCertificateValidation
        };
    }

    private static bool ServerCertificateValidation(HttpRequestMessage message, X509Certificate2? certificate, X509Chain? chain, SslPolicyErrors errors)
     => errors == SslPolicyErrors.None;

    private static bool IsAllowedOrigin(HttpRequestMessage message, string baseUrl)
     => message.RequestUri is not null && message.RequestUri.AbsoluteUri.StartsWith(baseUrl);
}