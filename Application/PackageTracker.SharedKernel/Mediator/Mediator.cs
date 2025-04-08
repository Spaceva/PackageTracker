using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PackageTracker.SharedKernel.Mediator;

internal class Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<Mediator> _logger = logger;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var notificationTypeName = typeof(TNotification).Name;
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
        if (!handlers.Any())
        {
            throw new InvalidOperationException($"No handler registered for notification type {notificationTypeName}");
        }

        _logger.LogDebug("Found {HandlersCount} handlers for {NotificationType}, processing...", handlers.Count(), notificationTypeName);
        var watch = new Stopwatch();
        watch.Start();
        Parallel.ForEach(handlers, async handler =>
        {
            await handler.Handle(notification, cancellationToken);
        });

        watch.Stop();
        _logger.LogDebug("Handled {NotificationType} in {Time}ms", notificationTypeName, watch.ElapsedMilliseconds);
        return Task.CompletedTask;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        var requestTypeName = typeof(TRequest).Name;
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>() ?? throw new InvalidOperationException($"No handler registered for request type {requestTypeName}");
        var watch = new Stopwatch();
        watch.Start();
        await handler.Handle(request, cancellationToken);
        watch.Stop();
        _logger.LogDebug("Handled {RequestType} in {Time}ms", requestTypeName, watch.ElapsedMilliseconds);
    }

    public async Task<TResponse> Query<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
    {
        var requestTypeName = typeof(TRequest).Name;
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>() ?? throw new InvalidOperationException($"No handler registered for request type {requestTypeName}");

        var watch = new Stopwatch();
        watch.Start();
        var result = await handler.Handle(request, cancellationToken);
        watch.Stop();
        _logger.LogDebug("Handled {RequestType} in {Time}ms", requestTypeName, watch.ElapsedMilliseconds);
        return result;
    }
}