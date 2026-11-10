namespace SystemX.Application.Abstractions.BackgroundJobs;

using System;

/// <summary>
/// Provides contextual metadata and runtime properties for an executing background job.
/// </summary>
/// <param name="JobId">The unique identifier of the background job execution, if available.</param>
/// <param name="JobName">The logical name of the background job.</param>
/// <param name="Attempt">The zero-based attempt index (0 for the initial execution).</param>
/// <param name="RecurringJobId">The recurring job identifier if triggered by a recurring schedule; otherwise <see langword="null"/>.</param>
/// <param name="TenantId">The tenant identifier active when the job was enqueued, if any.</param>
/// <param name="CorrelationId">The correlation identifier propagated from the scheduling context, if any.</param>
/// <param name="UserId">The user identifier active when the job was enqueued (informational only; jobs run as the system).</param>
/// <param name="EnqueuedAt">The timestamp when the job was originally enqueued or scheduled.</param>
public sealed record BackgroundJobContext(
    string? JobId,
    string JobName,
    int Attempt,
    string? RecurringJobId,
    string? TenantId,
    string? CorrelationId,
    string? UserId,
    DateTimeOffset EnqueuedAt);
