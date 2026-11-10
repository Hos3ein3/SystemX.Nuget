namespace SystemX.Application.Abstractions.Validation;

using System;
using System.Collections.Generic;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Results;

/// <summary>
/// Thrown by EnsureValidAsync and by validation decorators for handlers that do not return Result.
/// </summary>
public sealed class RequestValidationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestValidationException"/> class with a list of validation failures.
    /// </summary>
    /// <param name="failures">The collection of validation failures.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="failures"/> is <see langword="null"/>.</exception>
    public RequestValidationException(IReadOnlyList<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Failures = failures ?? throw new ArgumentNullException(nameof(failures));
    }

    /// <summary>
    /// Gets the list of validation failures.
    /// </summary>
    public IReadOnlyList<ValidationFailure> Failures { get; }
}
