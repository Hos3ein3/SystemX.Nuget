using SystemX.SharedKernel.Guards;

namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Abstract base record for strongly-typed GUID identifiers.
/// </summary>
public abstract record GuidId : StronglyTypedId<Guid>
{
    /// <summary>
    /// Initializes a new instance of <see cref="GuidId"/> with the specified non-empty GUID.
    /// </summary>
    /// <param name="value">The underlying non-empty GUID value.</param>
    protected GuidId(Guid value) : base(value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("The identifier cannot be Guid.Empty.", nameof(value));
        }
    }

    /// <summary>
    /// Formats the underlying GUID with the specified <see cref="UuidFormat"/>.
    /// </summary>
    /// <param name="format">The target UUID format.</param>
    /// <returns>The formatted string.</returns>
    public string ToString(UuidFormat format) => Uuid.ToString(Value, format);
}
