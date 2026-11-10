using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SystemX.Application.Abstractions.Events;
using SystemX.Application.Abstractions.Localization;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Messaging;

/// <summary>
/// Default implementation of <see cref="IIntegrationEventDispatcher"/> that routes received integration events
/// to their registered handlers within tenant, culture, and logging scopes.
/// </summary>
internal sealed partial class IntegrationEventDispatcher : IIntegrationEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Func<IntegrationEventDispatcher, object, IntegrationEventContext, CancellationToken, Task>> Invokers = new();

    private readonly IServiceProvider _serviceProvider;
    private readonly IntegrationEventDispatcherOptions _options;
    private readonly ILogger<IntegrationEventDispatcher> _logger;

    public IntegrationEventDispatcher(
        IServiceProvider serviceProvider,
        IntegrationEventDispatcherOptions options,
        ILogger<IntegrationEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DispatchAsync<TEvent>(TEvent integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        ArgumentNullException.ThrowIfNull(context);

        var handlers = _serviceProvider.GetServices<IIntegrationEventHandler<TEvent>>().ToList();
        if (handlers.Count == 0)
        {
            LogNoHandlers(_logger, typeof(TEvent).Name, context.MessageId);
            return;
        }

        using var logScope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["MessageId"] = context.MessageId,
            ["CorrelationId"] = context.CorrelationId,
            ["TenantId"] = context.TenantId
        });

        using var tenantScope = await BeginTenantScopeAsync(context, cancellationToken).ConfigureAwait(false);
        using var cultureScope = BeginCultureScope(context);

        List<Exception>? errors = null;
        foreach (var handler in handlers)
        {
            try
            {
                await handler.HandleAsync(integrationEvent, context, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (_options.ContinueOnHandlerFailure && ex is not OperationCanceledException)
            {
                (errors ??= []).Add(ex);
                LogHandlerFailed(_logger, ex, handler.GetType().Name, typeof(TEvent).Name);
            }
        }

        if (errors is { Count: > 0 })
        {
            throw new AggregateException($"{errors.Count} handler(s) failed for {typeof(TEvent).Name}.", errors);
        }
    }

    public Task DispatchAsync(object integrationEvent, IntegrationEventContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        ArgumentNullException.ThrowIfNull(context);

        var invoker = Invokers.GetOrAdd(integrationEvent.GetType(), static type =>
        {
            var method = typeof(IntegrationEventDispatcher)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(m => m.Name == nameof(DispatchAsync) && m.IsGenericMethodDefinition)
                .MakeGenericMethod(type);

            return (dispatcher, evt, ctx, ct) => (Task)method.Invoke(dispatcher, [evt, ctx, ct])!;
        });

        return invoker(this, integrationEvent, context, cancellationToken);
    }

    private async ValueTask<IDisposable?> BeginTenantScopeAsync(IntegrationEventContext context, CancellationToken cancellationToken)
    {
        var accessor = _serviceProvider.GetService<ITenantContext>() as ITenantContextAccessor
            ?? _serviceProvider.GetService<ITenantContextAccessor>();
        var store = _serviceProvider.GetService<ITenantStore>();

        if (accessor == null || store == null)
        {
            if (!string.IsNullOrEmpty(context.TenantId))
            {
                LogTenantServicesMissing(_logger, context.MessageId, context.TenantId);
            }
            return null;
        }

        if (string.IsNullOrWhiteSpace(context.TenantId))
        {
            if (_options.RequireTenantId)
            {
                throw new TenantNotResolvedException();
            }
            return null;
        }

        var tenantId = new TenantId(context.TenantId);
        var tenant = await store.FindByIdAsync(tenantId, cancellationToken).ConfigureAwait(false);
        if (tenant == null)
        {
            throw new TenantNotFoundException(tenantId);
        }

        if (!tenant.IsActive)
        {
            throw new TenantInactiveException(tenantId);
        }

        return accessor.BeginScope(tenant);
    }

    private IDisposable? BeginCultureScope(IntegrationEventContext context)
    {
        var currentCulture = _serviceProvider.GetService<ICurrentCulture>();
        if (currentCulture == null)
        {
            return null;
        }

        if (context.Headers != null &&
            context.Headers.TryGetValue(_options.CultureHeaderName, out var cultureName) &&
            !string.IsNullOrWhiteSpace(cultureName))
        {
            try
            {
                return currentCulture.BeginScope(cultureName);
            }
            catch (ArgumentException ex)
            {
                LogInvalidCulture(_logger, ex, cultureName, _options.CultureHeaderName, context.MessageId);
            }
        }

        return null;
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "No handlers for {EventType} (message {MessageId}).")]
    private static partial void LogNoHandlers(ILogger logger, string eventType, Guid messageId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Handler {Handler} failed for {EventType}.")]
    private static partial void LogHandlerFailed(ILogger logger, Exception ex, string handler, string eventType);

    [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Message {MessageId} specifies TenantId '{TenantId}', but multi-tenancy services are not registered.")]
    private static partial void LogTenantServicesMissing(ILogger logger, Guid messageId, string tenantId);

    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Invalid culture '{Culture}' in header '{HeaderName}' for message {MessageId}; ignoring.")]
    private static partial void LogInvalidCulture(ILogger logger, Exception ex, string culture, string headerName, Guid messageId);
}
