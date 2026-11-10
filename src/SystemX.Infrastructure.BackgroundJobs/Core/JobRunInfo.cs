namespace SystemX.Infrastructure.BackgroundJobs.Core;

/// <summary>
/// Contains execution identification details supplied by the background job provider.
/// </summary>
/// <param name="JobId">The unique provider identifier for this job run.</param>
/// <param name="Attempt">The zero-based retry attempt count.</param>
/// <param name="RecurringJobId">The recurring job identifier if this execution was initiated by a recurring schedule.</param>
internal sealed record JobRunInfo(string? JobId, int Attempt, string? RecurringJobId);
