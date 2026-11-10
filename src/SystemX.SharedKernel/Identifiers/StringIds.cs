using System.Diagnostics.CodeAnalysis;
using SystemX.SharedKernel.Results;

namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Provides static helper methods for creating, parsing, and converting strongly-typed string identifiers.
/// </summary>
public static class StringIds
{
    /// <summary>
    /// Creates an identifier instance from an existing non-empty string value.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="value">The underlying string value.</param>
    /// <returns>A new identifier instance.</returns>
    public static TId From<TId>(string value) where TId : StringId, IStringIdFactory<TId> =>
        TId.Create(value);

    /// <summary>
    /// Attempts to parse a string into a strongly-typed identifier.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="id">When successful, receives the identifier instance.</param>
    /// <returns><see langword="true"/> if parsed successfully; otherwise <see langword="false"/>.</returns>
    public static bool TryParse<TId>(string? text, [NotNullWhen(true)] out TId? id) where TId : StringId, IStringIdFactory<TId>
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            id = null;
            return false;
        }

        try
        {
            id = TId.Create(text);
            return true;
        }
        catch (ArgumentException)
        {
            id = null;
            return false;
        }
    }

    /// <summary>
    /// Parses a string into a strongly-typed identifier, throwing an exception on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <returns>The parsed identifier instance.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="text"/> is null, whitespace, or violates identifier rules.</exception>
    public static TId Parse<TId>(string text) where TId : StringId, IStringIdFactory<TId>
    {
        ArgumentNullException.ThrowIfNull(text);
        if (TryParse<TId>(text, out TId? id))
        {
            return id;
        }

        throw new FormatException($"String '{text}' was not recognized as a valid string identifier for {typeof(TId).Name}.");
    }

    /// <summary>
    /// Parses a string, returning a <see cref="Result{TId}"/> with a validation error on failure.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identifier type.</typeparam>
    /// <param name="text">The string to parse.</param>
    /// <param name="errorCode">The error code to use if parsing fails.</param>
    /// <returns>A successful result or a validation failure.</returns>
    public static Result<TId> ParseResult<TId>(string? text, string errorCode) where TId : StringId, IStringIdFactory<TId>
    {
        if (TryParse<TId>(text, out TId? id))
        {
            return Result<TId>.Success(id);
        }

        return Result.Failure<TId>(Error.Validation(errorCode, $"The provided value '{text}' is not a valid string identifier for {typeof(TId).Name}."));
    }
}
