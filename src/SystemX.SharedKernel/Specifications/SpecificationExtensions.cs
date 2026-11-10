namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Provides extension methods for evaluating specifications against in-memory collections.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// Evaluates a specification against an in-memory collection of entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="specification">The specification defining criteria, ordering, and paging.</param>
    /// <returns>The filtered, ordered, and paginated elements.</returns>
    public static IEnumerable<T> Evaluate<T>(this IEnumerable<T> source, ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(specification);

        IEnumerable<T> query = source;

        if (specification.Criteria != null)
        {
            Func<T, bool> predicate = specification.Criteria.Compile();
            query = query.Where(predicate);
        }

        if (specification.OrderBy.Count > 0)
        {
            IOrderedEnumerable<T>? ordered = null;
            for (int i = 0; i < specification.OrderBy.Count; i++)
            {
                OrderExpression<T> order = specification.OrderBy[i];
                Func<T, object> keySelector = order.KeySelector.Compile();

                if (i == 0)
                {
                    ordered = order.Descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
                }
                else
                {
                    ordered = order.Descending ? ordered!.ThenByDescending(keySelector) : ordered!.ThenBy(keySelector);
                }
            }

            if (ordered != null)
            {
                query = ordered;
            }
        }

        if (specification.Skip.HasValue)
        {
            query = query.Skip(specification.Skip.Value);
        }

        if (specification.Take.HasValue)
        {
            query = query.Take(specification.Take.Value);
        }

        return query;
    }

    /// <summary>
    /// Evaluates a specification with projection against an in-memory collection of entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="specification">The specification defining criteria, ordering, paging, and selector.</param>
    /// <returns>The filtered, ordered, paginated, and projected elements.</returns>
    public static IEnumerable<TResult> Evaluate<T, TResult>(this IEnumerable<T> source, ISpecification<T, TResult> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        IEnumerable<T> baseQuery = source.Evaluate((ISpecification<T>)specification);
        Func<T, TResult> selector = specification.Selector.Compile();
        return baseQuery.Select(selector);
    }
}
