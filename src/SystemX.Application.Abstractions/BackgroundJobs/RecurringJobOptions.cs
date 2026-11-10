namespace SystemX.Application.Abstractions.BackgroundJobs;

/// <summary>
/// Options for configuring a recurring background job.
/// </summary>
public sealed record RecurringJobOptions
{
    /// <summary>
    /// Gets the time zone identifier (IANA or Windows ID). Defaults to UTC when omitted or null.
    /// </summary>
    public string? TimeZoneId { get; init; }

    /// <summary>
    /// Gets the target queue name for providers supporting queues (e.g. Hangfire). Ignored by Quartz.
    /// </summary>
    public string? Queue { get; init; }

    /// <summary>
    /// Gets the maximum number of retry attempts for each recurring execution.
    /// </summary>
    public int? MaxRetries { get; init; }

    /// <summary>
    /// Gets a value indicating whether overlapping executions are permitted. Defaults to <see langword="false"/> (disallows concurrent execution).
    /// </summary>
    public bool AllowConcurrentExecution { get; init; }

    /// <summary>
    /// Gets a value indicating whether to capture and propagate the ambient tenant context. Defaults to <see langword="true"/>.
    /// </summary>
    public bool PropagateTenant { get; init; } = true;
}
