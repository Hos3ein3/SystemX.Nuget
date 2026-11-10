using System.Linq.Expressions;

namespace SystemX.SharedKernel.Specifications;

/// <summary>
/// Represents an ordering expression for an entity query.
/// </summary>
/// <typeparam name="T">The entity type being ordered.</typeparam>
/// <param name="KeySelector">The key selector expression.</param>
/// <param name="Descending">A value indicating whether the ordering is descending.</param>
public sealed record OrderExpression<T>(Expression<Func<T, object>> KeySelector, bool Descending);
