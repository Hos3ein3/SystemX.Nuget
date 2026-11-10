using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using SystemX.Application.Abstractions.MultiTenancy;
using SystemX.Application.Abstractions.Persistence;
using SystemX.Infrastructure.Persistence.EfCore.Configuration;
using SystemX.Infrastructure.Persistence.EfCore.Transactions;
using SystemX.SharedKernel.Auditing;
using SystemX.SharedKernel.Concurrency;
using SystemX.SharedKernel.Domain;
using SystemX.SharedKernel.Markers;
using SystemX.SharedKernel.MultiTenancy;

namespace SystemX.Infrastructure.Persistence.EfCore.Context;

/// <summary>
/// Abstract base EF Core DbContext implementing <see cref="IUnitOfWork"/> with multi-tenancy,
/// audit, soft delete, concurrency control, query filters, and domain event dispatch.
/// </summary>
public abstract class SystemXDbContext : DbContext, IUnitOfWork
{
    private static readonly MethodInfo BuildQueryFilterMethod =
        typeof(SystemXDbContext).GetMethod(nameof(BuildQueryFilter), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly SystemXDbContextDependencies _dependencies;
    private readonly HashSet<DataFilter> _disabledFilters = [];

    /// <summary>
    /// Initializes a new instance of <see cref="SystemXDbContext"/>.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    /// <param name="dependencies">The aggregated dependencies.</param>
    protected SystemXDbContext(DbContextOptions options, SystemXDbContextDependencies dependencies)
        : base(options)
    {
        _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        (EffectiveModel, TenantSchema, CurrentTenantId) = ResolveTenancy(dependencies);
    }

    /// <summary>
    /// Gets the tenancy model effective for the lifetime of this DbContext instance.
    /// </summary>
    internal TenancyModel EffectiveModel { get; }

    /// <summary>
    /// Gets the database schema resolved for the current tenant when using <see cref="TenancyModel.Schema"/>.
    /// </summary>
    internal string? TenantSchema { get; }

    /// <summary>
    /// Gets the active tenant identifier, or <see langword="null"/> when running in host mode.
    /// </summary>
    internal TenantId? CurrentTenantId { get; }

    /// <summary>
    /// Gets the cache key discriminator for this DbContext model.
    /// </summary>
    internal (TenancyModel Model, string? Schema) ModelCacheDiscriminator => (EffectiveModel, TenantSchema);

    /// <summary>
    /// Gets a value indicating whether the soft-delete query filter is active.
    /// </summary>
    internal bool SoftDeleteFilterEnabled => !_disabledFilters.Contains(DataFilter.SoftDelete);

    /// <summary>
    /// Gets a value indicating whether the archive query filter is active.
    /// </summary>
    internal bool ArchivedFilterEnabled => !_disabledFilters.Contains(DataFilter.Archived);

    /// <summary>
    /// Gets a value indicating whether the tenant isolation query filter is active.
    /// </summary>
    internal bool TenantFilterEnabled => !_disabledFilters.Contains(DataFilter.Tenant);

    /// <summary>
    /// Disables a global data filter until the returned scope handle is disposed.
    /// Disabling <see cref="DataFilter.Tenant"/> allows reading cross-tenant data and must be handled with care.
    /// </summary>
    /// <param name="filter">The filter to disable.</param>
    /// <returns>A disposable scope that re-enables the filter upon disposal.</returns>
    public IDisposable DisableFilter(DataFilter filter)
    {
        var wasEnabled = _disabledFilters.Add(filter);
        return new FilterScope(this, filter, wasEnabled);
    }

    /// <summary>
    /// Configures model entities. Override this method to configure entity mappings.
    /// Do not override <see cref="OnModelCreating(ModelBuilder)"/>.
    /// </summary>
    /// <param name="modelBuilder">The model builder being configured.</param>
    protected virtual void ConfigureModel(ModelBuilder modelBuilder)
    {
    }

    /// <inheritdoc />
    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureModel(modelBuilder);
        ApplyMarkerConfiguration(modelBuilder);
        ApplyTenancyModel(modelBuilder);
        ApplyQueryFilters(modelBuilder);
    }

    /// <inheritdoc />
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Conventions.Add(_ => new StronglyTypedIdConvention());
    }

    /// <inheritdoc />
    public bool HasActiveTransaction => Database.CurrentTransaction is not null;

    /// <inheritdoc />
    public async Task<ITransactionScope> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return new EfTransactionScope(transaction);
    }

    /// <summary>
    /// Synchronous SaveChanges is not supported because domain event dispatching is asynchronous.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new NotSupportedException("SystemXDbContext supports only SaveChangesAsync (domain events are async).");

    /// <summary>
    /// Synchronous SaveChanges is not supported because domain event dispatching is asynchronous.
    /// </summary>
    public override int SaveChanges() =>
        throw new NotSupportedException("SystemXDbContext supports only SaveChangesAsync (domain events are async).");

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(true, cancellationToken);

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        ApplySaveConventions();

        var emitters = GetDomainEventEmitters();
        var events = emitters.SelectMany(e => e.DomainEvents).ToList();

        var affected = await SaveCoreAsync(acceptAllChangesOnSuccess, cancellationToken);
        foreach (var emitter in emitters)
        {
            emitter.ClearDomainEvents();
        }

        var depth = 0;
        while (events.Count > 0)
        {
            if (++depth > _dependencies.Options.MaxDomainEventDepth)
            {
                throw new InvalidOperationException("Domain event dispatch exceeded MaxDomainEventDepth. Check for event loops.");
            }

            await _dependencies.DomainEventDispatcher.DispatchAsync(events, cancellationToken);

            if (ChangeTracker.HasChanges())
            {
                ChangeTracker.DetectChanges();
                ApplySaveConventions();
            }

            emitters = GetDomainEventEmitters();
            events = emitters.SelectMany(e => e.DomainEvents).ToList();

            if (ChangeTracker.HasChanges())
            {
                affected += await SaveCoreAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            foreach (var emitter in emitters)
            {
                emitter.ClearDomainEvents();
            }
        }

        return affected;
    }

    private async Task<int> SaveCoreAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw ToConcurrencyException(ex);
        }
    }

    private static ConcurrencyException ToConcurrencyException(DbUpdateConcurrencyException ex)
    {
        var entry = ex.Entries.Count > 0 ? ex.Entries[0] : null;
        var entityType = entry?.Entity.GetType().Name;
        var entityId = entry is not null ? GetPrimaryKeyValue(entry) : null;
        return new ConcurrencyException(entityType, entityId, ex);
    }

    private static string? GetPrimaryKeyValue(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null)
        {
            return null;
        }

        var values = key.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .Where(v => v is not null);
        var result = string.Join(",", values);
        return string.IsNullOrEmpty(result) ? null : result;
    }

    private List<IHasDomainEvents> GetDomainEventEmitters()
    {
        return ChangeTracker.Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();
    }

    private static (TenancyModel Model, string? Schema, TenantId? TenantId) ResolveTenancy(
        SystemXDbContextDependencies dependencies)
    {
        if (dependencies.Options.IgnoreTenancy)
        {
            return (TenancyModel.None, null, null);
        }

        var tenant = dependencies.TenantContext?.Tenant;
        if (tenant is null)
        {
            return (TenancyModel.None, null, null);
        }

        return tenant.Model switch
        {
            TenancyModel.Row => (TenancyModel.Row, null, tenant.Id),
            TenancyModel.Schema => ResolveSchemaTenancy(tenant, dependencies),
            TenancyModel.Database => (TenancyModel.Database, null, tenant.Id),
            TenancyModel.None => (TenancyModel.None, null, null),
            _ => throw new InvalidOperationException($"Unsupported tenancy model: {tenant.Model}")
        };
    }

    private static (TenancyModel Model, string? Schema, TenantId? TenantId) ResolveSchemaTenancy(
        TenantInfo tenant, SystemXDbContextDependencies dependencies)
    {
        var schema = dependencies.SchemaProvider?.GetSchema(tenant) ?? tenant.Id.Value;
        if (string.IsNullOrWhiteSpace(schema))
        {
            throw new InvalidOperationException(
                "Schema tenancy model requires a valid schema name from ITenantSchemaProvider.");
        }

        return (TenancyModel.Schema, schema, tenant.Id);
    }

    private void ApplySaveConventions()
    {
        var now = _dependencies.Clock.UtcNow;
        var user = _dependencies.CurrentUser?.UserId;

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyAddedConventions(entry, now, user);
                    break;
                case EntityState.Modified:
                    ApplyModifiedConventions(entry, now, user);
                    break;
                case EntityState.Deleted:
                    ApplyDeletedConventions(entry, now, user);
                    break;
            }
        }
    }

    private void ApplyAddedConventions(EntityEntry entry, DateTimeOffset now, string? user)
    {
        if (entry.Entity is ITenantScoped scoped)
        {
            if (scoped.TenantId is null)
            {
                if (CurrentTenantId is null)
                {
                    throw new TenantNotResolvedException("Cannot insert tenant-scoped entity without an active tenant.");
                }

                scoped.AssignTenant(CurrentTenantId);
            }
            else if (CurrentTenantId is not null && scoped.TenantId != CurrentTenantId)
            {
                throw new InvalidOperationException(
                    $"Entity tenant '{scoped.TenantId}' does not match current tenant '{CurrentTenantId}'.");
            }
        }

        if (entry.Entity is IMayHaveTenant optional)
        {
            if (optional.TenantId is null && CurrentTenantId is not null)
            {
                optional.AssignTenant(CurrentTenantId);
            }
            else if (optional.TenantId is not null && CurrentTenantId is not null && optional.TenantId != CurrentTenantId)
            {
                throw new InvalidOperationException(
                    $"Entity tenant '{optional.TenantId}' does not match current tenant '{CurrentTenantId}'.");
            }
        }

        if (entry.Entity is IHasCreationTime created)
        {
            created.SetCreationTime(now);
        }

        if (entry.Entity is IAuditable auditable)
        {
            auditable.SetCreated(now, user);
        }

        if (entry.Entity is IHasConcurrencyStamp stamped)
        {
            if (string.IsNullOrEmpty(stamped.ConcurrencyStamp))
            {
                stamped.RenewConcurrencyStamp();
            }
        }

        if (entry.Entity is IHasVersion versioned)
        {
            if (versioned.Version == 0)
            {
                versioned.IncrementVersion();
            }
        }
    }

    private void ApplyModifiedConventions(EntityEntry entry, DateTimeOffset now, string? user)
    {
        if (entry.Entity is IImmutableEntity)
        {
            throw new InvalidOperationException($"Entity '{entry.Entity.GetType().Name}' is immutable and cannot be modified.");
        }

        if (entry.Entity is ITenantScoped scoped)
        {
            if (CurrentTenantId is not null && scoped.TenantId != CurrentTenantId)
            {
                throw new InvalidOperationException($"Entity tenant '{scoped.TenantId}' does not match current tenant '{CurrentTenantId}'.");
            }

            if (entry.Property(nameof(ITenantScoped.TenantId)).IsModified)
            {
                throw new InvalidOperationException("TenantId cannot be changed once set.");
            }
        }

        if (entry.Entity is IMayHaveTenant)
        {
            if (entry.Property(nameof(IMayHaveTenant.TenantId)).IsModified)
            {
                throw new InvalidOperationException("TenantId cannot be changed once set.");
            }
        }

        if (entry.Entity is IHasModificationTime modified)
        {
            modified.SetModificationTime(now);
        }

        if (entry.Entity is IAuditable auditable)
        {
            auditable.SetModified(now, user);
        }

        if (entry.Entity is IHasConcurrencyStamp stamped)
        {
            stamped.RenewConcurrencyStamp();
        }

        if (entry.Entity is IHasVersion versioned)
        {
            versioned.IncrementVersion();
        }
    }

    private static void ApplyDeletedConventions(EntityEntry entry, DateTimeOffset now, string? user)
    {
        if (entry.Entity is IImmutableEntity)
        {
            throw new InvalidOperationException($"Entity '{entry.Entity.GetType().Name}' is immutable and cannot be deleted.");
        }

        if (entry.Entity is ISoftDeletable soft)
        {
            entry.State = EntityState.Modified;
            soft.MarkDeleted(now, user);
        }
    }

    private static void ApplyMarkerConfiguration(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.BaseType is not null || entityType.IsOwned())
            {
                continue;
            }

            var clrType = entityType.ClrType;

            if (typeof(IHasRowVersion).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(IHasRowVersion.RowVersion))
                    .IsRowVersion();
            }

            if (typeof(IHasVersion).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(IHasVersion.Version))
                    .IsConcurrencyToken();
            }

            if (typeof(IHasConcurrencyStamp).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
                    .IsConcurrencyToken()
                    .HasMaxLength(64);
            }

            if (typeof(ITenantScoped).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(ITenantScoped.TenantId))
                    .IsRequired();

                modelBuilder.Entity(clrType)
                    .HasIndex(nameof(ITenantScoped.TenantId));
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(clrType))
            {
                modelBuilder.Entity(clrType)
                    .HasIndex(nameof(IMayHaveTenant.TenantId));
            }
        }
    }

    private void ApplyTenancyModel(ModelBuilder modelBuilder)
    {
        if (EffectiveModel == TenancyModel.Schema && TenantSchema is not null)
        {
            modelBuilder.HasDefaultSchema(TenantSchema);

            if (_dependencies.Options.HostSchema is { } hostSchema)
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    if (entityType.BaseType is null && !entityType.IsOwned() && typeof(ISharedEntity).IsAssignableFrom(entityType.ClrType))
                    {
                        var tableName = entityType.GetTableName() ?? entityType.ClrType.Name;
                        modelBuilder.Entity(entityType.ClrType).ToTable(tableName, hostSchema);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "HostSchema must be configured on SystemXEfCoreOptions when using Schema tenancy model.");
            }
        }
    }

    private void ApplyQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.BaseType is not null || entityType.IsOwned())
            {
                continue;
            }

            var method = BuildQueryFilterMethod.MakeGenericMethod(entityType.ClrType);
#if NET10_0_OR_GREATER
#pragma warning disable CS0618
            var existingFilter = entityType.GetQueryFilter();
#pragma warning restore CS0618
#else
            var existingFilter = entityType.GetQueryFilter();
#endif
            var combinedFilter = (LambdaExpression?)method.Invoke(this, [existingFilter]);

            if (combinedFilter is not null)
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(combinedFilter);
            }
        }
    }

    private Expression<Func<TEntity, bool>>? BuildQueryFilter<TEntity>(LambdaExpression? existingFilter) where TEntity : class
    {
        Expression<Func<TEntity, bool>>? filter = existingFilter as Expression<Func<TEntity, bool>>;
        var type = typeof(TEntity);

        if (typeof(ISoftDeletable).IsAssignableFrom(type))
        {
            filter = FilterExpressionCombiner.And(filter,
                e => !SoftDeleteFilterEnabled || !EF.Property<bool>(e, nameof(ISoftDeletable.IsDeleted)));
        }

        if (typeof(IArchivable).IsAssignableFrom(type))
        {
            filter = FilterExpressionCombiner.And(filter,
                e => !ArchivedFilterEnabled || !EF.Property<bool>(e, nameof(IArchivable.IsArchived)));
        }

        if (EffectiveModel == TenancyModel.Row && !typeof(ISharedEntity).IsAssignableFrom(type))
        {
            if (typeof(ITenantScoped).IsAssignableFrom(type))
            {
                filter = FilterExpressionCombiner.And(filter,
                    e => !TenantFilterEnabled || EF.Property<TenantId>(e, nameof(ITenantScoped.TenantId)) == CurrentTenantId);
            }
            else if (typeof(IMayHaveTenant).IsAssignableFrom(type))
            {
                filter = FilterExpressionCombiner.And(filter,
                    e => !TenantFilterEnabled
                         || EF.Property<TenantId?>(e, nameof(IMayHaveTenant.TenantId)) == null
                         || EF.Property<TenantId?>(e, nameof(IMayHaveTenant.TenantId)) == CurrentTenantId);
            }
        }

        return filter;
    }

    private sealed class FilterScope : IDisposable
    {
        private readonly SystemXDbContext _context;
        private readonly DataFilter _filter;
        private readonly bool _wasEnabled;
        private bool _disposed;

        public FilterScope(SystemXDbContext context, DataFilter filter, bool wasEnabled)
        {
            _context = context;
            _filter = filter;
            _wasEnabled = wasEnabled;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_wasEnabled)
            {
                _context._disabledFilters.Remove(_filter);
            }
        }
    }
}
