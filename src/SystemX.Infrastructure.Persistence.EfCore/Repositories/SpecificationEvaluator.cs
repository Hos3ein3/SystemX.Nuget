using Microsoft.EntityFrameworkCore;
using SystemX.SharedKernel.Specifications;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

/// <summary>
/// Evaluates <see cref="ISpecification{T}"/> and builds an EF Core <see cref="IQueryable{T}"/>.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies criteria, includes, sorting, and pagination of a specification onto a queryable.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="input">The initial queryable (e.g. DbSet).</param>
    /// <param name="spec">The query specification.</param>
    /// <param name="criteriaOnly">When <see langword="true"/>, only criteria filters are applied (for counting).</param>
    /// <param name="applyPaging">When <see langword="true"/>, skip and take are applied.</param>
    /// <returns>The resulting queryable.</returns>
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> input,
        ISpecification<T> spec,
        bool criteriaOnly = false,
        bool applyPaging = true) where T : class
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(spec);

        var query = input;
        if (spec.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        if (spec.Criteria is not null)
        {
            query = query.Where(spec.Criteria);
        }

        if (criteriaOnly)
        {
            return query;
        }

        foreach (var include in spec.Includes)
        {
            query = query.Include(IncludePathBuilder.GetPath(include));
        }

        foreach (var path in spec.IncludeStrings)
        {
            query = query.Include(path);
        }

        if (spec.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        if (spec.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        IOrderedQueryable<T>? ordered = null;
        foreach (var order in spec.OrderBy)
        {
            ordered = ordered is null
                ? (order.Descending ? query.OrderByDescending(order.KeySelector) : query.OrderBy(order.KeySelector))
                : (order.Descending ? ordered.ThenByDescending(order.KeySelector) : ordered.ThenBy(order.KeySelector));
        }

        query = ordered ?? query;

        if (applyPaging)
        {
            if (spec.Skip is { } skip)
            {
                query = query.Skip(skip);
            }

            if (spec.Take is { } take)
            {
                query = query.Take(take);
            }
        }

        return query;
    }

    /// <summary>
    /// Applies criteria, projection, and pagination of a specification onto a queryable.
    /// </summary>
    /// <typeparam name="T">The source entity type.</typeparam>
    /// <typeparam name="TResult">The projected result type.</typeparam>
    /// <param name="input">The initial queryable.</param>
    /// <param name="spec">The projected specification.</param>
    /// <param name="applyPaging">When <see langword="true"/>, skip and take are applied.</param>
    /// <returns>The resulting projected queryable.</returns>
    public static IQueryable<TResult> GetQuery<T, TResult>(
        IQueryable<T> input,
        ISpecification<T, TResult> spec,
        bool applyPaging = true) where T : class
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(spec);

        var query = GetQuery(input, (ISpecification<T>)spec, criteriaOnly: false, applyPaging: false)
            .Select(spec.Selector);

        if (applyPaging)
        {
            if (spec.Skip is { } skip)
            {
                query = query.Skip(skip);
            }

            if (spec.Take is { } take)
            {
                query = query.Take(take);
            }
        }

        return query;
    }
}
