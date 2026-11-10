namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Abstract base record for strongly-typed identifiers.
/// </summary>
/// <typeparam name="TValue">The underlying identifier value type.</typeparam>
/// <param name="Value">The underlying identifier value.</param>
public abstract record StronglyTypedId<TValue>(TValue Value) : IStronglyTypedId<TValue>
    where TValue : notnull, IComparable<TValue>
{
    /// <inheritdoc />
    public override string ToString() => Value.ToString()!;
}
