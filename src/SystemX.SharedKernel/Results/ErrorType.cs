namespace SystemX.SharedKernel.Results;

/// <summary>The category of an error. The API layer maps it to an HTTP status.</summary>
public enum ErrorType
{
    /// <summary>Unexpected or general failure.</summary>
    Failure = 0,

    /// <summary>Input is invalid.</summary>
    Validation = 1,

    /// <summary>Resource was not found.</summary>
    NotFound = 2,

    /// <summary>State conflict, for example a duplicate.</summary>
    Conflict = 3,

    /// <summary>The caller is not authenticated.</summary>
    Unauthorized = 4,

    /// <summary>The caller is not allowed.</summary>
    Forbidden = 5
}
