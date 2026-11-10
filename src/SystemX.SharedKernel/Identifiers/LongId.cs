namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Abstract base record for strongly-typed 64-bit integer identifiers.
/// </summary>
public abstract record LongId : StronglyTypedId<long>
{
    /// <summary>
    /// Initializes a new instance of <see cref="LongId"/> with a positive 64-bit integer value.
    /// </summary>
    /// <param name="value">The underlying 64-bit integer value (must be greater than 0).</param>
    protected LongId(long value) : base(value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "The identifier must be greater than zero.");
        }
    }
}
