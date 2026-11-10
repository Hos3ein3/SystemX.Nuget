namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines operations for managing recurring background jobs.
/// </summary>
public interface IRecurringJobScheduler
{
    /// <summary>
    /// Gets the capabilities supported by this recurring scheduler provider.
    /// </summary>
    RecurringJobCapabilities Capabilities { get; }

    /// <summary>
    /// Registers or updates a recurring job with the specified identifier and schedule.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="recurringJobId">The unique recurring job identifier (1-100 alphanumeric, period, underscore, hyphen characters).</param>
    /// <param name="args">The arguments instance.</param>
    /// <param name="schedule">The recurring schedule (cron expression or interval).</param>
    /// <param name="options">Optional recurring job configuration.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the registration operation.</returns>
    Task AddOrUpdateAsync<TArgs>(string recurringJobId, TArgs args, RecurringSchedule schedule, RecurringJobOptions? options = null, CancellationToken cancellationToken = default) where TArgs : class;

    /// <summary>
    /// Removes a recurring job by identifier.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns><see langword="true"/> if the job existed and was removed; otherwise <see langword="false"/>.</returns>
    Task<bool> RemoveAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether a recurring job with the specified identifier exists.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns><see langword="true"/> if the job exists; otherwise <see langword="false"/>.</returns>
    Task<bool> ExistsAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Immediately triggers execution of an existing recurring job out-of-band.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the trigger operation.</returns>
    Task TriggerAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses execution of a recurring job.
    /// Supported by Quartz. Unsupported by Hangfire (throws <see cref="NotSupportedException"/>).
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the pause operation.</returns>
    Task PauseAsync(string recurringJobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes execution of a previously paused recurring job.
    /// Supported by Quartz. Unsupported by Hangfire (throws <see cref="NotSupportedException"/>).
    /// </summary>
    /// <param name="recurringJobId">The recurring job identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the resume operation.</returns>
    Task ResumeAsync(string recurringJobId, CancellationToken cancellationToken = default);
}
