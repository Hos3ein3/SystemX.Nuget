namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Defines a factory contract for creating strongly-typed string identifiers without reflection.
/// </summary>
/// <typeparam name="TSelf">The concrete identifier type.</typeparam>
public interface IStringIdFactory<TSelf> where TSelf : StringId
{
    /// <summary>
    /// Creates an instance of <typeparamref name="TSelf"/> with the specified string value.
    /// </summary>
    /// <param name="value">The underlying string value.</param>
    /// <returns>A new identifier instance.</returns>
    static abstract TSelf Create(string value);
}
