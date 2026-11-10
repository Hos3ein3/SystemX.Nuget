namespace SystemX.Infrastructure.BackgroundJobs.Core;

using System.Text.Json;

/// <summary>
/// Configuration options for SystemX background jobs infrastructure.
/// </summary>
public sealed class BackgroundJobsOptions
{
    /// <summary>
    /// Gets or sets the JSON serialization options used for serializing and deserializing job arguments.
    /// Defaults to Web options. Add custom converters (e.g. strongly-typed ID converters) here.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets or sets the default maximum retry attempts when not specified per-job. Defaults to 3.
    /// </summary>
    public int DefaultMaxRetries { get; set; } = 3;
}
