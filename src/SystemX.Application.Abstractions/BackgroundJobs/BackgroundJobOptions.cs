namespace SystemX.Application.Abstractions.BackgroundJobs;

/// <summary>
/// Options for configuring the execution and context propagation of a background job.
/// </summary>
public sealed record BackgroundJobOptions
{
    /// <summary>
    /// Gets the target queue name for providers supporting multiple queues (e.g. Hangfire). Ignored by Quartz.
    /// </summary>
    public string? Queue { get; init; }

    /// <summary>
    /// Gets the maximum number of retry attempts for this job. When omitted, the provider default is used.
    /// </summary>
    public int? MaxRetries { get; init; }

    /// <summary>
    /// Gets an explicit correlation identifier. When omitted, the ambient activity or trace identifier is captured.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Gets a value indicating whether to capture and propagate the ambient tenant context. Defaults to <see langword="true"/>.
    /// </summary>
    public bool PropagateTenant { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether to capture and propagate the ambient culture and UI culture. Defaults to <see langword="true"/>.
    /// </summary>
    public bool PropagateCulture { get; init; } = true;
}
