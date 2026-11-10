namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System;
using System.Text.Json;

/// <summary>
/// Represents the serialized envelope persisted in scheduler storage.
/// </summary>
internal sealed record JobEnvelope(
    string JobName,
    string ArgsJson,
    string? TenantId,
    string? Culture,
    string? CorrelationId,
    string? UserId,
    DateTimeOffset EnqueuedAt,
    int MaxRetries,
    bool AllowConcurrentExecution,
    int Version = 1)
{
    public string ToJson() => JsonSerializer.Serialize(this, EnvelopeJson.Options);

    public static JobEnvelope FromJson(string json) =>
        JsonSerializer.Deserialize<JobEnvelope>(json, EnvelopeJson.Options)
        ?? throw new InvalidOperationException("The job envelope is empty.");
}
