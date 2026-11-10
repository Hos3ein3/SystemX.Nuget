namespace SystemX.SharedKernel.Results;

/// <summary>
/// Represents an error containing one or more validation failures.
/// </summary>
public sealed record ValidationError : Error
{
    /// <summary>
    /// Gets the list of property validation failures.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Failures { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationError"/> with the specified collection of failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public ValidationError(IEnumerable<ValidationFailure> failures)
        : base("Validation.Failed", "One or more validation failures occurred.", ErrorType.Validation)
    {
        ArgumentNullException.ThrowIfNull(failures);
        Failures = failures.ToList().AsReadOnly();
    }
}
