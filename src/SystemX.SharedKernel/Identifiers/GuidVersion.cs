namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Specifies the UUID/GUID version.
/// </summary>
public enum GuidVersion
{
    /// <summary>
    /// Version 7 UUID (time-ordered, RFC 9562).
    /// </summary>
    V7 = 7,

    /// <summary>
    /// Version 4 UUID (random).
    /// </summary>
    V4 = 4
}
