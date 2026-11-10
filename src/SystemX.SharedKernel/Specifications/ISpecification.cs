using System.Linq.Expressions;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Encapsulates query logic for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the filtering criteria expression, or <see langword="null"/> if no filter is applied.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the navigation property include expressions.
    /// </summary>
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the string-based navigation property includes.
    /// </summary>
    IReadOnlyList<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the order-by expressions.
    /// </summary>
    IReadOnlyList<OrderExpression<T>> OrderBy { get; }

    /// <summary>
    /// Gets the number of elements to skip, or <see langword="null"/> if paging is not applied.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the maximum number of elements to return, or <see langword="null"/> if paging is not applied.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets a value indicating whether query results should not be tracked by the underlying change tracker.
    /// </summary>
    bool AsNoTracking { get; }

    /// <summary>
    /// Gets a value indicating whether related collections should be split into multiple SQL queries.
    /// </summary>
    bool AsSplitQuery { get; }

    /// <summary>
    /// Gets a value indicating whether global query filters should be ignored.
    /// </summary>
    bool IgnoreQueryFilters { get; }
}
