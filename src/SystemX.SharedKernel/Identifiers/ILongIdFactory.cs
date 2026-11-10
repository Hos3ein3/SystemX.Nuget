namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Defines a factory contract for creating strongly-typed 64-bit integer identifiers without reflection.
/// </summary>
/// <typeparam name="TSelf">The concrete identifier type.</typeparam>
public interface ILongIdFactory<TSelf> where TSelf : LongId
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> with the specified 64-bit integer value.
    /// </summary>
    /// <param name="value">The underlying 64-bit integer value.</param>
    /// <returns>A new identifier instance.</returns>
    static abstract TSelf Create(long value);
}
