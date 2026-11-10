using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SystemX.Infrastructure.Persistence.EfCore.Repositories;

internal static class IdPredicate
{
    private static readonly MethodInfo EfPropertyMethod =
        typeof(EF).GetMethod(nameof(EF.Property), BindingFlags.Public | BindingFlags.Static)!;

    public static Expression<Func<TEntity, bool>> Build<TEntity, TId>(TId id)
        where TEntity : class
        where TId : notnull
    {
        var holder = new IdHolder<TId>(id);
        var parameter = Expression.Parameter(typeof(TEntity), "e");

        var closedPropertyMethod = EfPropertyMethod.MakeGenericMethod(typeof(TId));
        var idProperty = Expression.Call(closedPropertyMethod, parameter, Expression.Constant("Id"));
        var holderConstant = Expression.Constant(holder);
        var holderValue = Expression.Property(holderConstant, nameof(IdHolder<TId>.Value));

        var equality = Expression.Equal(idProperty, holderValue);
        return Expression.Lambda<Func<TEntity, bool>>(equality, parameter);
    }

    private sealed record IdHolder<TId>(TId Value);
}
