using FluentValidation.Results;
using SharedKernelValidationFailure = SystemX.SharedKernel.Results.ValidationFailure;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides extension methods for converting FluentValidation results to SystemX results.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Converts a FluentValidation <see cref="ValidationResult"/> to a read-only list of <see cref="SharedKernelValidationFailure"/>.
    /// </summary>
    /// <param name="result">The FluentValidation result.</param>
    /// <param name="includeErrorCodes">Whether to include custom error codes or use the default code.</param>
    /// <returns>A list of SystemX validation failures.</returns>
    public static IReadOnlyList<SharedKernelValidationFailure> ToFailures(this ValidationResult result, bool includeErrorCodes = true)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsValid || result.Errors.Count == 0)
        {
            return Array.Empty<SharedKernelValidationFailure>();
        }

        List<SharedKernelValidationFailure> failures = new(result.Errors.Count);
        foreach (FluentValidation.Results.ValidationFailure error in result.Errors)
        {
            string code = includeErrorCodes && !string.IsNullOrWhiteSpace(error.ErrorCode)
                ? error.ErrorCode
                : "Validation.Failed";
            failures.Add(new SharedKernelValidationFailure(error.PropertyName, code, error.ErrorMessage));
        }

        return failures.AsReadOnly();
    }
}
