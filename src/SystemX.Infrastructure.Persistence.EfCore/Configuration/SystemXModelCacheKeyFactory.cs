using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SystemX.Infrastructure.Persistence.EfCore.Context;

namespace SystemX.Infrastructure.Persistence.EfCore.Configuration;

/// <summary>
/// Model cache key factory that includes multi-tenancy discriminator (model and schema) in the cache key.
/// </summary>
public sealed class SystemXModelCacheKeyFactory : IModelCacheKeyFactory
{
    /// <inheritdoc />
    public object Create(DbContext context, bool designTime)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context is SystemXDbContext c
            ? (context.GetType(), c.ModelCacheDiscriminator, designTime)
            : (context.GetType(), designTime);
    }
}
