using System.Diagnostics.CodeAnalysis;
using SystemX.SharedKernel.Results;

namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Provides static helper methods for creating, parsing, and converting strongly-typed GUID identifiers.
/// </summary>
public static class GuidIds
{
    /// <summary>
    /// Creates a new identifier with a newly generated UUID (defaults to UUIDv7).
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="version">The UUID version to generate.</param>
    /// <returns>A new identifier instance.</returns>
    public static TId New<TId>(GuidVersion version = GuidVersion.V7) where TId : GuidId, IGuidIdFactory<TId> =>
        TId.Create(Uuid.New(version));

    /// <summary>
    /// Creates an identifier instance from an existing GUID value.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="value">The underlying GUID value.</param>
    /// <returns>A new identifier instance.</returns>
    public static TId From<TId>(Guid value) where TId : GuidId, IGuidIdFactory<TId> =>
        TId.Create(value);

    /// <summary>
    /// Attempts to parse a string representation of a GUID into a strongly-typed identifier.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="id">When successful, receives the identifier instance.</param>
    /// <returns><see langword="true"/> if parsed successfully; otherwise <see langword="false"/>.</returns>
    public static bool TryParse<TId>(string? text, [NotNullWhen(true)] out TId? id) where TId : GuidId, IGuidIdFactory<TId>
    {
        if (Uuid.TryParse(text, out Guid guid) && guid != Guid.Empty)
        {
            id = TId.Create(guid);
            return true;
        }

        id = null;
        return false;
    }

    /// <summary>
    /// Parses a string representation of a GUID into a strongly-typed identifier, throwing an exception on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <returns>The parsed identifier instance.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="text"/> is not a valid non-empty GUID.</exception>
    public static TId Parse<TId>(string text) where TId : GuidId, IGuidIdFactory<TId>
    {
        ArgumentNullException.ThrowIfNull(text);
        if (TryParse<TId>(text, out TId? id))
        {
            return id;
        }

        throw new FormatException($"String '{text}' was not recognized as a valid {typeof(TId).Name}.");
    }

    /// <summary>
    /// Parses a string representation of a GUID, returning a <see cref="Result{TId}"/> with a validation error on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="errorCode">The error code to use if parsing fails.</param>
    /// <returns>A successful result or a validation failure.</returns>
    public static Result<TId> ParseResult<TId>(string? text, string errorCode) where TId : GuidId, IGuidIdFactory<TId>
    {
        if (TryParse<TId>(text, out TId? id))
        {
            return Result<TId>.Success(id);
        }

        return Result.Failure<TId>(Error.Validation(errorCode, $"The provided value '{text}' is not a valid {typeof(TId).Name}."));
    }
}
