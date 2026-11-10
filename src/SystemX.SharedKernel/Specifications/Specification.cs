using System.Linq.Expressions;
using SystemX.SharedKernel.Pagination;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Abstract base implementation of <see cref="ISpecification{T}"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<OrderExpression<T>> _orderBy = [];

    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();

    /// <inheritdoc />
    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();

    /// <inheritdoc />
    IReadOnlyList<OrderExpression<T>> ISpecification<T>.OrderBy => _orderBy.AsReadOnly();

    /// <summary>
    /// Gets the list of order expressions configured for this specification.
    /// </summary>
    public IReadOnlyList<OrderExpression<T>> OrderExpressions => _orderBy.AsReadOnly();

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <summary>
    /// Gets a value indicating whether query results are tracked by the change tracker.
    /// Defaults to <see langword="true"/> (no tracking).
    /// </summary>
    public bool AsNoTracking { get; private set; } = true;

    /// <inheritdoc />
    public bool AsSplitQuery { get; private set; }

    /// <inheritdoc />
    public bool IgnoreQueryFilters { get; private set; }

    /// <summary>
    /// Applies a filtering criteria expression to the specification.
    /// If criteria already exists, it is combined using logical AND.
    /// </summary>
    /// <param name="criteria">The filter predicate.</param>
    protected void Where(Expression<Func<T, bool>> criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        Criteria = Criteria == null ? criteria : ExpressionCombiner.And(Criteria, criteria);
    }

    /// <summary>
    /// Includes a related entity using a property selector expression.
    /// </summary>
    /// <param name="include">The include property expression.</param>
    protected void Include(Expression<Func<T, object>> include)
    {
        ArgumentNullException.ThrowIfNull(include);
        _includes.Add(include);
    }

    /// <summary>
    /// Includes a related entity navigation path using a dot-delimited string.
    /// </summary>
    /// <param name="includeString">The navigation path.</param>
    protected void Include(string includeString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(includeString);
        _includeStrings.Add(includeString);
    }

    /// <summary>
    /// Adds ascending primary ordering.
    /// </summary>
    /// <param name="keySelector">The key selector expression.</param>
    protected void OrderBy(Expression<Func<T, object>> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        _orderBy.Add(new OrderExpression<T>(keySelector, Descending: false));
    }

    /// <summary>
    /// Adds descending primary ordering.
    /// </summary>
    /// <param name="keySelector">The key selector expression.</param>
    protected void OrderByDescending(Expression<Func<T, object>> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        _orderBy.Add(new OrderExpression<T>(keySelector, Descending: true));
    }

    /// <summary>
    /// Adds subsequent ascending ordering.
    /// </summary>
    /// <param name="keySelector">The key selector expression.</param>
    protected void ThenBy(Expression<Func<T, object>> keySelector) => OrderBy(keySelector);

    /// <summary>
    /// Adds subsequent descending ordering.
    /// </summary>
    /// <param name="keySelector">The key selector expression.</param>
    protected void ThenByDescending(Expression<Func<T, object>> keySelector) => OrderByDescending(keySelector);

    /// <summary>
    /// Applies pagination parameters from a <see cref="PageRequest"/>.
    /// </summary>
    /// <param name="pageRequest">The pagination request.</param>
    protected void ApplyPaging(PageRequest pageRequest)
    {
        ArgumentNullException.ThrowIfNull(pageRequest);
        Skip = pageRequest.Skip;
        Take = pageRequest.PageSize;
    }

    /// <summary>
    /// Applies skip and take counts for pagination.
    /// </summary>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The maximum number of elements to return.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip < 0 ? 0 : skip;
        Take = take < 1 ? 1 : take;
    }

    /// <summary>
    /// Configures the query to disable change tracking.
    /// </summary>
    protected void NoTracking() => AsNoTracking = true;

    /// <summary>
    /// Configures the query to enable change tracking.
    /// </summary>
    protected void Tracking() => AsNoTracking = false;

    /// <summary>
    /// Configures the query to split collections across multiple queries.
    /// </summary>
    protected void SplitQuery() => AsSplitQuery = true;

    /// <summary>
    /// Configures the query to bypass global query filters (such as soft delete or tenant filters).
    /// </summary>
    protected void WithoutQueryFilters() => IgnoreQueryFilters = true;

    /// <summary>
    /// Combines two specifications using logical AND.
    /// </summary>
    public static Specification<T> operator &(Specification<T> left, Specification<T> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        CompositeSpecification<T> result = new();
        result.CopyFrom(left);
        if (right.Criteria != null)
        {
            result.Where(right.Criteria);
        }

        foreach (Expression<Func<T, object>> inc in right.Includes)
        {
            result.Include(inc);
        }

        foreach (string incStr in right.IncludeStrings)
        {
            result.Include(incStr);
        }

        return result;
    }

    /// <summary>
    /// Combines two specifications using logical OR on their criteria.
    /// </summary>
    public static Specification<T> operator |(Specification<T> left, Specification<T> right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        CompositeSpecification<T> result = new();
        if (left.Criteria != null && right.Criteria != null)
        {
            result.SetCriteria(ExpressionCombiner.Or(left.Criteria, right.Criteria));
        }
        else if (left.Criteria != null)
        {
            result.SetCriteria(left.Criteria);
        }
        else if (right.Criteria != null)
        {
            result.SetCriteria(right.Criteria);
        }

        return result;
    }

    /// <summary>
    /// Negates the criteria of the specified specification.
    /// </summary>
    public static Specification<T> operator !(Specification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        CompositeSpecification<T> result = new();
        result.CopyFrom(specification);
        if (specification.Criteria != null)
        {
            result.SetCriteria(ExpressionCombiner.Not(specification.Criteria));
        }

        return result;
    }

    internal void CopyFrom(Specification<T> source)
    {
        Criteria = source.Criteria;
        _includes.AddRange(source.Includes);
        _includeStrings.AddRange(source.IncludeStrings);
        _orderBy.AddRange(source.OrderExpressions);
        Skip = source.Skip;
        Take = source.Take;
        AsNoTracking = source.AsNoTracking;
        AsSplitQuery = source.AsSplitQuery;
        IgnoreQueryFilters = source.IgnoreQueryFilters;
    }

    internal void SetCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
}

internal sealed class CompositeSpecification<T> : Specification<T>
{
}
