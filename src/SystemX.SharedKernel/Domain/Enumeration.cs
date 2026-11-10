using System.Reflection;

namespace SystemX.SharedKernel.Domain;

/// <summary>Base class for type-safe enumerations that can hold behavior.</summary>
/// <typeparam name="TEnum">The derived type itself.</typeparam>
public abstract class Enumeration<TEnum> : IEquatable<TEnum>, IComparable<TEnum>
    where TEnum : Enumeration<TEnum>
{
    private static readonly Lazy<IReadOnlyDictionary<int, TEnum>> Items = new(() =>
        typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.FieldType == typeof(TEnum))
            .Select(f => (TEnum)f.GetValue(null)!)
            .ToDictionary(e => e.Value));

    /// <summary>Creates an item.</summary>
    protected Enumeration(int value, string name)
    {
        Value = value;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>Gets the numeric value.</summary>
    public int Value { get; }

    /// <summary>Gets the name.</summary>
    public string Name { get; }

    /// <summary>Gets all items.</summary>
    public static IReadOnlyCollection<TEnum> GetAll() => Items.Value.Values.ToList();

    /// <summary>Finds an item by value, or returns null.</summary>
    public static TEnum? FromValue(int value) => Items.Value.GetValueOrDefault(value);

    /// <summary>Finds an item by name (case-insensitive), or returns null.</summary>
    public static TEnum? FromName(string name) =>
        Items.Value.Values.FirstOrDefault(e => string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));

    /// <inheritdoc />
    public bool Equals(TEnum? other) => other is not null && GetType() == other.GetType() && Value == other.Value;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is TEnum other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Value;

    /// <inheritdoc />
    public int CompareTo(TEnum? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override string ToString() => Name;

    /// <summary>Less than operator.</summary>
    public static bool operator <(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null ? right is not null : left.CompareTo(right as TEnum) < 0;

    /// <summary>Less than or equal operator.</summary>
    public static bool operator <=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null || left.CompareTo(right as TEnum) <= 0;

    /// <summary>Greater than operator.</summary>
    public static bool operator >(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is not null && left.CompareTo(right as TEnum) > 0;

    /// <summary>Greater than or equal operator.</summary>
    public static bool operator >=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null ? right is null : left.CompareTo(right as TEnum) >= 0;

    /// <summary>Equality operator.</summary>
    public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right) =>
        left is null ? right is null : left.Equals(right as TEnum);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) => !(left == right);
}
