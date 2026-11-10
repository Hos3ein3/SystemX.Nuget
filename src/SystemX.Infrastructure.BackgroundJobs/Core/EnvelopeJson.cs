namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System.Text.Json;

/// <summary>
/// Provides internal fixed JSON serializer options for job envelope storage.
/// </summary>
internal static class EnvelopeJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
}
