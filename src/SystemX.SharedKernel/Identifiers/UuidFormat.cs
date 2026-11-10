namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Specifies the string formatting options for UUIDs.
/// </summary>
public enum UuidFormat
{
    /// <summary>
    /// Standard 36-character hyphenated format: 00000000-0000-0000-0000-000000000000 (D).
    /// </summary>
    Default,

    /// <summary>
    /// 32-character compact hexadecimal format with no hyphens: 00000000000000000000000000000000 (N).
    /// </summary>
    Compact,

    /// <summary>
    /// 38-character format enclosed in braces: {00000000-0000-0000-0000-000000000000} (B).
    /// </summary>
    Braces,

    /// <summary>
    /// 22-character URL-safe Base64 format without padding (+ replaced with -, / replaced with _).
    /// </summary>
    Base64Url
}
