namespace SystemX.SharedKernel.Identifiers;

/// <summary>
/// Abstract base record for strongly-typed string identifiers.
/// </summary>
public abstract record StringId : StronglyTypedId<string>
{
    /// <summary>
    /// Gets the optional maximum allowed length for the string identifier, or null if unrestricted.
    /// </summary>
    protected virtual int? MaxLength => null;

    /// <summary>
    /// Initializes a new instance of <see cref="StringId"/> with the specified non-empty string.
    /// </summary>
    /// <param name="value">The underlying string value.</param>
    protected StringId(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The identifier cannot be null or whitespace.", nameof(value));
        }

        if (MaxLength.HasValue && value.Length > MaxLength.Value)
        {
            throw new ArgumentException($"The identifier exceeds the maximum length of {MaxLength.Value}.", nameof(value));
        }
    }
}
