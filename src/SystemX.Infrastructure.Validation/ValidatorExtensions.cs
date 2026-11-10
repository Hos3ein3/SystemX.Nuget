using FluentValidation;
using FluentValidation.Results;
using SystemX.SharedKernel.Results;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides extension methods on <see cref="IValidator{T}"/>.
/// </summary>
public static class ValidatorExtensions
{
    /// <summary>
    /// Validates an instance and returns a <see cref="Result"/> containing a <see cref="ValidationError"/> on failure.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="validator">The validator instance.</param>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>A successful result if valid; otherwise a failed result with <see cref="ValidationError"/>.</returns>
    public static Result ValidateToResult<T>(this IValidator<T> validator, T instance)
    {
        ArgumentNullException.ThrowIfNull(validator);

        ValidationResult result = validator.Validate(instance);
        if (result.IsValid)
        {
            return Result.Success();
        }

        return Result.Failure(Error.Validation(result.ToFailures()));
    }
}
