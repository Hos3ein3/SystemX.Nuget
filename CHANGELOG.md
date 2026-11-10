# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- SystemX.SharedKernel: redesigned multi-tenancy models (`TenancyModel.None`, `Row`, `Schema`, `Database`), added strongly-typed `TenantId`, `TenantInfo` with validation and redacted connection string formatting, and tenant markers (`ITenantScoped`, `IMayHaveTenant`, `ISharedEntity`, `TenantInfoExtensions`).
- SystemX.SharedKernel: added optimistic concurrency primitives (`IConcurrencySafe`, `IHasRowVersion`, `IHasVersion`, `IHasConcurrencyStamp`, `ConcurrencyException`, `VersionedAggregateRoot<TId>`, `StampedAggregateRoot<TId>`).
- SystemX.SharedKernel: added entity markers (`IHasCreationTime`, `IHasModificationTime`, `IArchivable`, `IActivatable`, `IOrderable`, `IImmutableEntity`, `INotAudited`, `IReadModel`, `IOwnedBy<TOwnerId>`, `ITranslatable<TTranslation>`, `ITranslation`, `TranslationExtensions.Pick`).
- SystemX.Application.Abstractions: added multi-tenancy interfaces and transport-neutral models (`ITenantContext`, `ITenantContextAccessor`, `ITenantStore`, `ITenantResolutionStrategy`, `TenantResolutionContext`, `ITenantConnectionResolver`, `ITenantSchemaProvider`, `ITenantProvisioner`, `TenantNotResolvedException`, `TenantNotFoundException`, `TenantInactiveException`, and `ICacheService`).
- SystemX.Infrastructure.MultiTenancy: added resolution strategies (`Header`, `Claim`, `Host`, `Route`, `Query`), stores (`InMemoryTenantStore`, `ConfigurationTenantStore`, `CachedTenantStore`), `TenantContextAccessor`, `TenantResolver`, `DefaultTenantConnectionResolver`, `DefaultTenantSchemaProvider`, ASP.NET Core middleware (`UseSystemXMultiTenancy`), and DI extensions (`AddSystemXMultiTenancy`).
- SystemX.Infrastructure.Persistence.EfCore: implemented multi-tenancy model conventions (Row query filters, Schema default schema and `TenantModelCacheKeyFactory`, Database `TenantDbContextOptionsConfigurator`), save changes interceptor (`PersistenceSaveChangesInterceptor`) managing tenant assignments, timestamps, optimistic concurrency increment/renewal, and immutable entity protection.
- SystemX.SharedKernel: added strongly-typed identifiers (`IStronglyTypedId<TValue>`, `StronglyTypedId<TValue>`, `GuidId`, `IntId`, `LongId`, `StringId`, `Uuid`, `IGuidIdFactory<T>`, `IIntIdFactory<T>`, `ILongIdFactory<T>`, `IStringIdFactory<T>`, `GuidIds`, `IntIds`, `LongIds`, `StringIds`).
- SystemX.SharedKernel: added specifications pattern (`ISpecification<T>`, `ISpecification<T, TResult>`, `Specification<T>`, `Specification<T, TResult>`, `OrderExpression<T>`, `ByIdSpecification<T, TId>`, `SpecificationExtensions.Evaluate`).
- SystemX.SharedKernel: added pagination primitives (`PageRequest`, `PagedList<T>`, `CursorPageRequest`, `CursorPagedList<T>`).
- SystemX.SharedKernel: added validation error primitives (`ValidationFailure`, `ValidationError`, `Error.Validation(failures)`).
- SystemX.Contracts: added pagination DTOs (`PagedRequest`, `PagedResponse<T>`, `CursorPagedResponse<T>`).
- SystemX.Application.Abstractions: added repository interfaces (`IReadRepository<T>`, `IRepository<T>`) and CQRS message interfaces (`ICommand`, `ICommand<TResult>`, `IQuery<TResult>`, `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>`).
- SystemX.Contracts.Validation: new package providing FluentValidation validators for contract DTOs (`PagedRequestValidator`, `ContractsValidationAssemblyMarker`).
- SystemX.Infrastructure.Validation: new package providing validation pipeline, CQRS decorators (`AddSystemXValidation`), options (`ValidationOptions`), exception (`RequestValidationException`), and rule extensions (`MustBeValidGuid`, `MustBeGuidV7`, `MustBePositiveId`, `NotEmptyGuid`, `ValidStringId`, `ValidateToResult`).
- SystemX.Infrastructure.Serialization: new package providing System.Text.Json converters (`StronglyTypedIdJsonConverter<TId, TValue>`, `StronglyTypedIdJsonConverterFactory`), type converters (`StronglyTypedIdTypeConverter`, `StronglyTypedIdTypeDescriptor`), and DI extensions (`AddSystemXConverters`, `AddSystemXSerialization`).
- SystemX.SharedKernel: expanded time abstractions (`IClock`, `SystemClock`, `ClockExtensions`, `ClockTimer`, `TimeInterval`, `DateRange`).
- SystemX.SharedKernel: added Domain primitives (`IEntity<TId>`, `Entity<TId>`, `IAggregateRoot`, `AggregateRoot<TId>`, `IHasDomainEvents`, `IDomainEvent`, `DomainEvent`, `ValueObject`, `Enumeration<TEnum>`, `IBusinessRule`, `BusinessRuleValidationException`, `DomainException`).
- SystemX.SharedKernel: added Auditing contracts and base entities (`IAuditable`, `AuditableEntity<TId>`, `AuditableAggregateRoot<TId>`, `ISoftDeletable`).
- SystemX.SharedKernel: added functional Result pattern and pipeline extensions (`Result`, `Result<T>`, `Error`, `ErrorType`, `ResultExtensions` with `Map`, `Bind`, `Ensure`, `Match`).
- SystemX.SharedKernel: added defensive argument assertions (`Guard`).
- SystemX.SharedKernel: added testable time abstractions (`IClock`, `SystemClock`).
- Initial project scaffolding for SystemX NuGet library suite.
- Central Package Management and Directory.Build.props setup.
- Automated release and CI workflows with MinVer.

### Changed
- **BREAKING** SystemX.SharedKernel: replaced `ITenantScoped` `string TenantId` property with strongly-typed `TenantId TenantId` and added `AssignTenant(TenantId)` method.
- SystemX.SharedKernel: made `Error` a non-sealed record to support polymorphic error types such as `ValidationError` without breaking existing call sites.
- SystemX.SharedKernel: `AuditableEntity<TId>` and `AuditableAggregateRoot<TId>` now implement `IHasCreationTime` and `IHasModificationTime`.
