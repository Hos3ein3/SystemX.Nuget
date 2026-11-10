namespace SystemX.Infrastructure.Messaging;

/// <summary>
/// Configuration options for the integration event dispatcher.
/// </summary>
public sealed class IntegrationEventDispatcherOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether all handlers run even if one fails.
    /// When true, failures are aggregated and thrown together as an <see cref="AggregateException"/> at the end.
    /// Default is false (first failure stops execution and throws).
    /// </summary>
    public bool ContinueOnHandlerFailure { get; set; }

    /// <summary>
    /// Gets or sets the message header name that carries the culture of the message.
    /// Default is "x-culture".
    /// </summary>
    public string CultureHeaderName { get; set; } = "x-culture";

    /// <summary>
    /// Gets or sets a value indicating whether a message without a TenantId is rejected if multi-tenancy services are registered.
    /// Default is false.
    /// </summary>
    public bool RequireTenantId { get; set; }
}
