using FluentValidation.Results;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides extension methods for converting FluentValidation results to SystemX results.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Converts a FluentValidation <see cref="ValidationResult"/> to a read-only list of <see cref="SharedKernel.Results.ValidationFailure"/>.
    /// </summary>
    /// <param name="result">The FluentValidation result.</param>
    /// <returns>A list of SystemX validation failures.</returns>
    public static IReadOnlyList<SharedKernel.Results.ValidationFailure> ToFailures(this ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsValid || result.Errors.Count == 0)
        {
            return Array.Empty<SharedKernel.Results.ValidationFailure>();
        }

        List<SharedKernel.Results.ValidationFailure> failures = new(result.Errors.Count);
        foreach (FluentValidation.Results.ValidationFailure error in result.Errors)
        {
            string code = string.IsNullOrWhiteSpace(error.ErrorCode) ? "Validation.Failed" : error.ErrorCode;
            failures.Add(new SharedKernel.Results.ValidationFailure(error.PropertyName, code, error.ErrorMessage));
        }

        return failures.AsReadOnly();
    }
}
