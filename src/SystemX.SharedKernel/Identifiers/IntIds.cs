using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SystemX.SharedKernel.Results;

namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Provides static helper methods for creating, parsing, and converting strongly-typed 32-bit integer identifiers.
/// </summary>
public static class IntIds
{
    /// <summary>
    /// Creates an identifier instance from an existing positive integer value.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="value">The underlying integer value (must be greater than 0).</param>
    /// <returns>A new identifier instance.</returns>
    public static TId From<TId>(int value) where TId : IntId, IIntIdFactory<TId> =>
        TId.Create(value);

    /// <summary>
    /// Attempts to parse a string representation of an integer into a strongly-typed identifier.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="id">When successful, receives the identifier instance.</param>
    /// <returns><see langword="true"/> if parsed successfully; otherwise <see langword="false"/>.</returns>
    public static bool TryParse<TId>(string? text, [NotNullWhen(true)] out TId? id) where TId : IntId, IIntIdFactory<TId>
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) && value > 0)
        {
            id = TId.Create(value);
            return true;
        }

        id = null;
        return false;
    }

    /// <summary>
    /// Parses a string representation of an integer into a strongly-typed identifier, throwing an exception on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <returns>The parsed identifier instance.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="text"/> is not a valid positive integer.</exception>
    public static TId Parse<TId>(string text) where TId : IntId, IIntIdFactory<TId>
    {
        ArgumentNullException.ThrowIfNull(text);
        if (TryParse<TId>(text, out TId? id))
        {
            return id;
        }

        throw new FormatException($"String '{text}' was not recognized as a valid positive integer for {typeof(TId).Name}.");
    }

    /// <summary>
    /// Parses a string representation of an integer, returning a <see cref="Result{TId}"/> with a validation error on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="errorCode">The error code to use if parsing fails.</param>
    /// <returns>A successful result or a validation failure.</returns>
    public static Result<TId> ParseResult<TId>(string? text, string errorCode) where TId : IntId, IIntIdFactory<TId>
    {
        if (TryParse<TId>(text, out TId? id))
        {
            return Result<TId>.Success(id);
        }

        return Result.Failure<TId>(Error.Validation(errorCode, $"The provided value '{text}' is not a valid positive integer for {typeof(TId).Name}."));
    }
}
