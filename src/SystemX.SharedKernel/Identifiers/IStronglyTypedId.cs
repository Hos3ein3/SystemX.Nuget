namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Defines a strongly-typed identifier wrapping a primitive value.
/// </summary>
/// <typeparam name="TValue">The underlying primitive value type.</typeparam>
public interface IStronglyTypedId<out TValue> where TValue : notnull
{
    /// <summary>
    /// Gets the underlying primitive identifier value.
    /// </summary>
    TValue Value { get; }
}
