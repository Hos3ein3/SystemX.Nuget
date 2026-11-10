using System.Text.RegularExpressions;
using FluentValidation;
using SystemX.Contracts.Pagination;

namespace SystemX.Contracts.Validation;

/// <summary>
/// Validator for <see cref="PagedRequest"/> DTOs.
/// </summary>
public sealed class PagedRequestValidator : AbstractValidator<PagedRequest>
{
    private static readonly Regex SafeSortByRegex = new("^[a-zA-Z0-9_.]+$", RegexOptions.Compiled);

    /// <summary>
    /// Initializes validation rules for <see cref="PagedRequest"/>.
    /// </summary>
    public PagedRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode("Pagination.InvalidPage")
            .WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithErrorCode("Pagination.InvalidPageSize")
            .WithMessage("PageSize must be between 1 and 200.");

        RuleFor(x => x.SortBy)
            .MaximumLength(50)
            .WithErrorCode("Pagination.SortByTooLong")
            .WithMessage("SortBy property name cannot exceed 50 characters.")
            .Must(sortBy => sortBy == null || SafeSortByRegex.IsMatch(sortBy))
            .WithErrorCode("Pagination.UnsafeSortBy")
            .WithMessage("SortBy contains invalid characters.");
    }
}
