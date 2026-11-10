using System.Buffers.Binary;
using System.Security.Cryptography;
using SystemX.SharedKernel.Time;

namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Provides utilities for generating, formatting, and parsing UUIDs (RFC 9562 UUIDv7 and UUIDv4).
/// </summary>
public static class Uuid
{
    /// <summary>
    /// Creates a new GUID using the specified version (defaults to UUIDv7).
    /// </summary>
    /// <param name="version">The GUID version to generate.</param>
    /// <returns>A new <see cref="Guid"/>.</returns>
    public static Guid New(GuidVersion version = GuidVersion.V7) =>
        version switch
        {
            GuidVersion.V7 => NewV7(),
            GuidVersion.V4 => NewV4(),
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, "Unsupported GUID version.")
        };

    /// <summary>
    /// Creates a new random UUIDv4.
    /// </summary>
    /// <returns>A new <see cref="Guid"/>.</returns>
    public static Guid NewV4() => Guid.NewGuid();

    /// <summary>
    /// Creates a new time-ordered UUIDv7 based on current UTC time.
    /// </summary>
    /// <returns>A new <see cref="Guid"/>.</returns>
    public static Guid NewV7()
    {
#if NET9_0_OR_GREATER
        return Guid.CreateVersion7();
#else
        return NewV7(DateTimeOffset.UtcNow);
#endif
    }

    /// <summary>
    /// Creates a new time-ordered UUIDv7 based on the specified timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp to embed in the UUID.</param>
    /// <returns>A new <see cref="Guid"/>.</returns>
    public static Guid NewV7(DateTimeOffset timestamp)
    {
#if NET9_0_OR_GREATER
        return Guid.CreateVersion7(timestamp);
#else
        long unixMs = timestamp.ToUnixTimeMilliseconds();
        if (unixMs < 0)
        {
            unixMs = 0;
        }

        byte[] bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);

        // 48-bit unix ms timestamp in big-endian
        bytes[0] = (byte)(unixMs >> 40);
        bytes[1] = (byte)(unixMs >> 32);
        bytes[2] = (byte)(unixMs >> 24);
        bytes[3] = (byte)(unixMs >> 16);
        bytes[4] = (byte)(unixMs >> 8);
        bytes[5] = (byte)unixMs;

        // Version 7 in bits 4-7 of byte 6 (0111_xxxx)
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70);

        // Variant 1 (RFC 4122/9562) in bits 6-7 of byte 8 (10xx_xxxx)
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes, bigEndian: true);
#endif
    }

    /// <summary>
    /// Creates a new time-ordered UUIDv7 using time from the specified clock.
    /// </summary>
    /// <param name="clock">The clock providing UTC time.</param>
    /// <returns>A new <see cref="Guid"/>.</returns>
    public static Guid NewV7(IClock clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        return NewV7(clock.UtcNow);
    }

    /// <summary>
    /// Formats a GUID according to the specified format.
    /// </summary>
    /// <param name="guid">The GUID to format.</param>
    /// <param name="format">The target format.</param>
    /// <returns>The formatted string.</returns>
    public static string ToString(Guid guid, UuidFormat format) =>
        format switch
        {
            UuidFormat.Default => guid.ToString("D"),
            UuidFormat.Compact => guid.ToString("N"),
            UuidFormat.Braces => guid.ToString("B"),
            UuidFormat.Base64Url => ToBase64Url(guid),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported UUID format.")
        };

    /// <summary>
    /// Parses a string representation of a GUID in Default, Compact, Braces, or Base64Url format.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <param name="guid">When successful, receives the parsed <see cref="Guid"/>.</param>
    /// <returns><see langword="true"/> if parsed successfully; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? text, out Guid guid)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            guid = Guid.Empty;
            return false;
        }

        string trimmed = text.Trim();

        // 22 chars without punctuation is Base64Url
        if (trimmed.Length == 22 && TryParseBase64Url(trimmed, out guid))
        {
            return true;
        }

        return Guid.TryParse(trimmed, out guid);
    }

    private static string ToBase64Url(Guid guid)
    {
        byte[] bytes = guid.ToByteArray(bigEndian: true);
        string base64 = Convert.ToBase64String(bytes);
        // 16 bytes = 24 base64 chars with 2 padding '='
        return base64[..22]
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static bool TryParseBase64Url(string input, out Guid guid)
    {
        guid = Guid.Empty;
        if (input.Length != 22)
        {
            return false;
        }

        string base64 = input
            .Replace('-', '+')
            .Replace('_', '/') + "==";

        try
        {
            byte[] bytes = Convert.FromBase64String(base64);
            if (bytes.Length != 16)
            {
                return false;
            }

            guid = new Guid(bytes, bigEndian: true);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
