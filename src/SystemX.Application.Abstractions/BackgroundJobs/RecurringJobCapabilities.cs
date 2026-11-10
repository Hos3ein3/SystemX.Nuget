namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;

/// <summary>
/// Defines capabilities supported by a recurring job scheduler implementation.
/// </summary>
[Flags]
public enum RecurringJobCapabilities
{
    /// <summary>No capabilities.</summary>
    None = 0,

    /// <summary>5-field cron expression scheduling (supported by Hangfire and Quartz).</summary>
    Cron = 1,

    /// <summary>Fixed interval scheduling (supported by Quartz; Hangfire supports divisors of 60m or 24h).</summary>
    Interval = 2,

    /// <summary>Time zone-aware recurring schedules (supported by Hangfire and Quartz).</summary>
    TimeZones = 4,

    /// <summary>Pausing recurring jobs (supported by Quartz; unsupported by Hangfire).</summary>
    Pause = 8,

    /// <summary>Triggering immediate out-of-band execution of a recurring job (supported by Hangfire and Quartz).</summary>
    TriggerNow = 16,

    /// <summary>Preventing concurrent overlapping executions of the same recurring job (supported by Hangfire and Quartz).</summary>
    ConcurrencyControl = 32
}
