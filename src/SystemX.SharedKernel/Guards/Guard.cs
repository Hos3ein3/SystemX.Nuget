using System.Runtime.CompilerServices;

namespace SystemX.SharedKernel.Guards;

/// <summary>Argument checks that throw when a value is invalid.</summary>
public static class Guard
{
    /// <summary>Throws <see cref="ArgumentNullException"/> when the value is null.</summary>
    public static T NotNull<T>(T? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : class =>
        value ?? throw new ArgumentNullException(name);

    /// <summary>Throws when the string is null, empty or whitespace.</summary>
    public static string NotNullOrWhiteSpace(string? value, [CallerArgumentExpression(nameof(value))] string? name = null) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be null, empty or whitespace.", name)
            : value;

    /// <summary>Throws when the value equals its default (for example Guid.Empty or 0).</summary>
    public static T NotDefault<T>(T value, [CallerArgumentExpression(nameof(value))] string? name = null) =>
        EqualityComparer<T>.Default.Equals(value, default!)
            ? throw new ArgumentException("Value cannot be the default value.", name)
            : value;

    /// <summary>Throws when the collection is null or empty.</summary>
    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T>? value, [CallerArgumentExpression(nameof(value))] string? name = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }

        return value.Any() ? value : throw new ArgumentException("Collection cannot be empty.", name);
    }

    /// <summary>Throws when the value is outside the inclusive range.</summary>
    public static T InRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : IComparable<T> =>
        value.CompareTo(min) < 0 || value.CompareTo(max) > 0
            ? throw new ArgumentOutOfRangeException(name, value, $"Value must be between {min} and {max}.")
            : value;

    /// <summary>Throws when the number is zero or negative.</summary>
    public static T Positive<T>(T value, [CallerArgumentExpression(nameof(value))] string? name = null)
        where T : IComparable<T>, System.Numerics.INumberBase<T> =>
        value.CompareTo(T.Zero) <= 0
            ? throw new ArgumentOutOfRangeException(name, value, "Value must be greater than zero.")
            : value;

    /// <summary>Throws when the condition is false.</summary>
    public static void Requires(bool condition, string message, [CallerArgumentExpression(nameof(condition))] string? name = null)
    {
        if (!condition)
        {
            throw new ArgumentException(message, name);
        }
    }
}
