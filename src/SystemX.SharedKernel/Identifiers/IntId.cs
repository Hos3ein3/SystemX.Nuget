namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Abstract base record for strongly-typed 32-bit integer identifiers.
/// </summary>
public abstract record IntId : StronglyTypedId<int>
{
    /// <summary>
    /// Initializes a new instance of <see cref="IntId"/> with a positive integer value.
    /// </summary>
    /// <param name="value">The underlying integer value (must be greater than 0).</param>
    protected IntId(int value) : base(value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "The identifier must be greater than zero.");
        }
    }
}
