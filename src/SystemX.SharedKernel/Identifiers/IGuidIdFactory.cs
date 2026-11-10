namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Defines a factory contract for creating strongly-typed GUID identifiers without reflection.
/// </summary>
/// <typeparam name="TSelf">The concrete identifier type.</typeparam>
public interface IGuidIdFactory<TSelf> where TSelf : GuidId
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> with the specified GUID value.
    /// </summary>
    /// <param name="value">The underlying GUID value.</param>
    /// <returns>A new identifier instance.</returns>
    static abstract TSelf Create(Guid value);
}
