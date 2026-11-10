namespace SystemX.SharedKernel.Results;

/// <summary>
/// Represents a failure for a specific property during validation.
/// </summary>
/// <param name="PropertyName">The name of the validated property.</param>
/// <param name="Code">The validation error code.</param>
/// <param name="Message">The human-readable validation error message.</param>
public sealed record ValidationFailure(string PropertyName, string Code, string Message);
