namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Defines a factory contract for creating strongly-typed 32-bit integer identifiers without reflection.
/// </summary>
/// <typeparam name="TSelf">The concrete identifier type.</typeparam>
public interface IIntIdFactory<TSelf> where TSelf : IntId
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> with the specified integer value.
    /// </summary>
    /// <param name="value">The underlying integer value.</param>
    /// <returns>A new identifier instance.</returns>
    static abstract TSelf Create(int value);
}
