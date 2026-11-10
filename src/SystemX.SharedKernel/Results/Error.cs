namespace SystemX.SharedKernel.Results;

/// <summary>Describes why an operation failed.</summary>
/// <param name="Code">A stable code, for example "User.NotFound".</param>
/// <param name="Description">A human readable description.</param>
/// <param name="Type">The error category.</param>
public sealed record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    /// <summary>Represents "no error". Used by successful results.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Error for a null value.</summary>
    public static readonly Error NullValue = new("General.Null", "A null value was provided.");

    /// <summary>Creates a failure error.</summary>
    public static Error Failure(string code, string description) => new(code, description);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

    /// <summary>Creates an unauthorized error.</summary>
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);

    /// <summary>Creates a forbidden error.</summary>
    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);
}
