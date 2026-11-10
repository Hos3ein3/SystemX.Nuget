namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines operations for enqueuing and scheduling one-off background jobs.
/// </summary>
public interface IBackgroundJobScheduler
{
    /// <summary>
    /// Gets the capabilities supported by this scheduler provider.
    /// </summary>
    BackgroundJobCapabilities Capabilities { get; }

    /// <summary>
    /// Enqueues a job for immediate execution.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="args">The arguments instance.</param>
    /// <param name="options">Optional execution and context propagation settings.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The unique provider identifier for the enqueued job.</returns>
    Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobOptions? options = null, CancellationToken cancellationToken = default) where TArgs : class;

    /// <summary>
    /// Schedules a job for execution after a specified time delay.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="args">The arguments instance.</param>
    /// <param name="delay">The delay duration before the job should run.</param>
    /// <param name="options">Optional execution and context propagation settings.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The unique provider identifier for the scheduled job.</returns>
    Task<string> ScheduleAsync<TArgs>(TArgs args, TimeSpan delay, BackgroundJobOptions? options = null, CancellationToken cancellationToken = default) where TArgs : class;

    /// <summary>
    /// Schedules a job for execution at a specific point in time.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="args">The arguments instance.</param>
    /// <param name="runAt">The exact timestamp when the job should run.</param>
    /// <param name="options">Optional execution and context propagation settings.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The unique provider identifier for the scheduled job.</returns>
    Task<string> ScheduleAsync<TArgs>(TArgs args, DateTimeOffset runAt, BackgroundJobOptions? options = null, CancellationToken cancellationToken = default) where TArgs : class;

    /// <summary>
    /// Schedules a continuation job that executes when its parent job finishes.
    /// Supported by Hangfire. Unsupported by Quartz (throws <see cref="NotSupportedException"/>).
    /// </summary>
    /// <typeparam name="TArgs">The job arguments type.</typeparam>
    /// <param name="parentJobId">The identifier of the parent job.</param>
    /// <param name="args">The arguments instance.</param>
    /// <param name="options">Optional execution and context propagation settings.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The unique provider identifier for the continuation job.</returns>
    Task<string> ContinueWithAsync<TArgs>(string parentJobId, TArgs args, BackgroundJobOptions? options = null, CancellationToken cancellationToken = default) where TArgs : class;

    /// <summary>
    /// Deletes or cancels a scheduled or enqueued job.
    /// Supported by Hangfire and Quartz.
    /// </summary>
    /// <param name="jobId">The unique provider identifier of the job.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns><see langword="true"/> if the job was successfully found and deleted; otherwise <see langword="false"/>.</returns>
    Task<bool> DeleteAsync(string jobId, CancellationToken cancellationToken = default);
}
