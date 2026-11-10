namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;

/// <summary>
/// Defines capabilities supported by a background job scheduler implementation.
/// </summary>
[Flags]
public enum BackgroundJobCapabilities
{
    /// <summary>No capabilities.</summary>
    None = 0,

    /// <summary>Immediate fire-and-forget execution (supported by Hangfire and Quartz).</summary>
    FireAndForget = 1,

    /// <summary>Delayed scheduling by time interval or specific time (supported by Hangfire and Quartz).</summary>
    Delayed = 2,

    /// <summary>Continuation jobs executed upon parent completion (supported by Hangfire; unsupported by Quartz).</summary>
    Continuations = 4,

    /// <summary>Explicit cancellation or deletion of scheduled jobs (supported by Hangfire and Quartz).</summary>
    Delete = 8,

    /// <summary>Named execution queues (supported by Hangfire; unsupported by Quartz).</summary>
    Queues = 16,

    /// <summary>Configurable automatic retries upon failure (supported by Hangfire and Quartz).</summary>
    AutomaticRetries = 32
}
