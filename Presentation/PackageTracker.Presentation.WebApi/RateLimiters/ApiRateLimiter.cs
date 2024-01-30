using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace PackageTracker.WebHost.RateLimiters;

public class ApiRateLimiter : IRateLimiterPolicy<string>
{
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected =>
        async (rejectedContext, cancellationToken) =>
    {
        rejectedContext.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await rejectedContext.HttpContext.Response.WriteAsync("Too many requests", cancellationToken);
    };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "default";
        return RateLimitPartition.GetTokenBucketLimiter(key,
                key => new TokenBucketRateLimiterOptions { TokenLimit = 50, TokensPerPeriod = 25, ReplenishmentPeriod = TimeSpan.FromSeconds(3), AutoReplenishment = true, QueueLimit = 0 });
    }
}
