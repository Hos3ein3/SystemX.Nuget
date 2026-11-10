using FluentValidation;
using SystemX.SharedKernel.Identifiers;

namespace SystemX.Infrastructure.Validation;

/// <summary>
/// Provides reusable FluentValidation rule builder extensions for identifiers and primitives.
/// </summary>
public static class ValidationRuleExtensions
{
    /// <summary>
    /// Validates that a string is a valid UUID in Default, Compact, Braces, or Base64Url format.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidGuid<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .Must(text => Uuid.TryParse(text, out Guid guid) && guid != Guid.Empty)
            .WithMessage("'{PropertyName}' must be a valid GUID.")
            .WithErrorCode("Validation.InvalidGuid");
    }

    /// <summary>
    /// Validates that a GUID is a version 7 UUID (time-ordered).
    /// </summary>
    public static IRuleBuilderOptions<T, Guid> MustBeGuidV7<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .Must(guid =>
            {
                if (guid == Guid.Empty) return false;
                byte[] bytes = guid.ToByteArray(bigEndian: true);
                // Version 7 has 0111 in upper nibble of byte 6
                return (bytes[6] >> 4) == 7;
            })
            .WithMessage("'{PropertyName}' must be a Version 7 UUID.")
            .WithErrorCode("Validation.NotGuidV7");
    }

    /// <summary>
    /// Validates that a string represents a version 7 UUID.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeGuidV7<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .Must(text =>
            {
                if (!Uuid.TryParse(text, out Guid guid) || guid == Guid.Empty) return false;
                byte[] bytes = guid.ToByteArray(bigEndian: true);
                return (bytes[6] >> 4) == 7;
            })
            .WithMessage("'{PropertyName}' must be a Version 7 UUID.")
            .WithErrorCode("Validation.NotGuidV7");
    }

    /// <summary>
    /// Validates that an integer identifier is positive (greater than zero).
    /// </summary>
    public static IRuleBuilderOptions<T, int> MustBePositiveId<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .GreaterThan(0)
            .WithMessage("'{PropertyName}' must be greater than zero.")
            .WithErrorCode("Validation.PositiveIdRequired");
    }

    /// <summary>
    /// Validates that a 64-bit integer identifier is positive (greater than zero).
    /// </summary>
    public static IRuleBuilderOptions<T, long> MustBePositiveId<T>(this IRuleBuilder<T, long> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .GreaterThan(0L)
            .WithMessage("'{PropertyName}' must be greater than zero.")
            .WithErrorCode("Validation.PositiveIdRequired");
    }

    /// <summary>
    /// Validates that a GUID is not <see cref="Guid.Empty"/>.
    /// </summary>
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .Must(guid => guid != Guid.Empty)
            .WithMessage("'{PropertyName}' must not be empty.")
            .WithErrorCode("Validation.EmptyGuid");
    }

    /// <summary>
    /// Validates that a string identifier is not null or whitespace and does not exceed the specified maximum length.
    /// </summary>
    /// <typeparam name="T">The root type being validated.</typeparam>
    /// <param name="ruleBuilder">The rule builder.</param>
    /// <param name="maxLength">The maximum allowed string length.</param>
    public static IRuleBuilderOptions<T, string?> ValidStringId<T>(this IRuleBuilder<T, string?> ruleBuilder, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(ruleBuilder);
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' must not be empty.")
            .WithErrorCode("Validation.EmptyStringId")
            .MaximumLength(maxLength)
            .WithMessage($"'{{PropertyName}}' must not exceed {maxLength} characters.")
            .WithErrorCode("Validation.StringIdTooLong");
    }
}
