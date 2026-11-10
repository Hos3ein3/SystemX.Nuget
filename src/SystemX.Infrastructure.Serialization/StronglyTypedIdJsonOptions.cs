using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Serialization;

/// <summary>
/// Options for configuring JSON serialization of strongly-typed identifiers.
/// </summary>
public sealed class StronglyTypedIdJsonOptions
{
    /// <summary>
    /// Gets or sets the UUID format used when serializing strongly-typed GUID identifiers.
    /// Defaults to <see cref="UuidFormat.Default"/> (hyphenated 36 characters).
    /// </summary>
    public UuidFormat WriteFormat { get; set; } = UuidFormat.Default;
}
