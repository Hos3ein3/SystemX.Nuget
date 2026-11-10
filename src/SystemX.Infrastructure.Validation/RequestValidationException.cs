using SystemX.SharedKernel.Results;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Exception thrown when a command or query validation fails and the handler return type is not a <see cref="Result"/>.
/// </summary>
public sealed class RequestValidationException : Exception
{
    /// <summary>
    /// Gets the list of validation failures.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Failures { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="RequestValidationException"/> with the given failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    public RequestValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures occurred.")
    {
        ArgumentNullException.ThrowIfNull(failures);
        Failures = failures.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RequestValidationException"/> with a message and inner exception.
    /// </summary>
    public RequestValidationException(string message, Exception innerException) : base(message, innerException)
    {
        Failures = Array.Empty<ValidationFailure>();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="RequestValidationException"/> with a message.
    /// </summary>
    public RequestValidationException(string message) : base(message)
    {
        Failures = Array.Empty<ValidationFailure>();
    }

    /// <summary>
    /// Initializes a new default instance of <see cref="RequestValidationException"/>.
    /// </summary>
    public RequestValidationException() : base("One or more validation failures occurred.")
    {
        Failures = Array.Empty<ValidationFailure>();
    }
}
