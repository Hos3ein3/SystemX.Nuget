namespace SystemX.Application.Abstractions.Validation;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SystemX.SharedKernel.Results;

/// <summary>
/// Validates objects (commands, queries, DTOs, messages). The implementation decides which rules run.
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates an instance. Returns a failed Result with a ValidationError, or success. No validators = success.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful <see cref="Result"/> if valid or if no validators are found; otherwise a failed <see cref="Result"/> with a validation error.</returns>
    Task<Result> ValidateAsync<T>(T instance, CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    /// Same as ValidateAsync but uses the runtime type of the instance (for transport consumers).
    /// </summary>
    /// <param name="instance">The object instance to validate.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A successful <see cref="Result"/> if valid or if no validators are found; otherwise a failed <see cref="Result"/> with a validation error.</returns>
    Task<Result> ValidateObjectAsync(object instance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all failures (empty when valid).
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A read-only collection of <see cref="ValidationFailure"/> instances.</returns>
    Task<IReadOnlyList<ValidationFailure>> GetFailuresAsync<T>(T instance, CancellationToken cancellationToken = default)
        where T : notnull;

    /// <summary>
    /// Throws RequestValidationException when invalid.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous validation check.</returns>
    /// <exception cref="RequestValidationException">Thrown when validation failures are detected.</exception>
    Task EnsureValidAsync<T>(T instance, CancellationToken cancellationToken = default)
        where T : notnull;
}
