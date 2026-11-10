using FluentValidation;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Options for configuring validation pipelines.
/// </summary>
public sealed class ValidationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether validation stops on the first failure.
    /// Default is <see langword="false"/>.
    /// </summary>
    public bool StopOnFirstFailure { get; set; }

    /// <summary>
    /// Gets or sets the cascade mode for rule execution.
    /// Default is <see cref="CascadeMode.Continue"/>.
    /// </summary>
    public CascadeMode CascadeMode { get; set; } = CascadeMode.Continue;

    /// <summary>
    /// Gets or sets a value indicating whether error codes from validators are included.
    /// Default is <see langword="true"/>.
    /// </summary>
    public bool IncludeErrorCodes { get; set; } = true;
}
