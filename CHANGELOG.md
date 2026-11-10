# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- SystemX.SharedKernel: added strongly-typed identifiers (`IStronglyTypedId<TValue>`, `StronglyTypedId<TValue>`, `GuidId`, `IntId`, `LongId`, `StringId`, `Uuid`, `IGuidIdFactory<T>`, `IIntIdFactory<T>`, `ILongIdFactory<T>`, `IStringIdFactory<T>`, `GuidIds`, `IntIds`, `LongIds`, `StringIds`).
- SystemX.SharedKernel: added specifications pattern (`ISpecification<T>`, `ISpecification<T, TResult>`, `Specification<T>`, `Specification<T, TResult>`, `OrderExpression<T>`, `ByIdSpecification<T, TId>`, `SpecificationExtensions.Evaluate`).
- SystemX.SharedKernel: added pagination primitives (`PageRequest`, `PagedList<T>`, `CursorPageRequest`, `CursorPagedList<T>`).
- SystemX.SharedKernel: added validation error primitives (`ValidationFailure`, `ValidationError`, `Error.Validation(failures)`).
- SystemX.Contracts: added pagination DTOs (`PagedRequest`, `PagedResponse<T>`, `CursorPagedResponse<T>`).
- SystemX.Application.Abstractions: added repository interfaces (`IReadRepository<T>`, `IRepository<T>`) and CQRS message interfaces (`ICommand`, `ICommand<TResult>`, `IQuery<TResult>`, `ICommandHandler<TCommand>`, `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>`).
- SystemX.Contracts.Validation: new package providing FluentValidation validators for contract DTOs (`PagedRequestValidator`, `ContractsValidationAssemblyMarker`).
- SystemX.Infrastructure.Validation: new package providing validation pipeline, CQRS decorators (`AddSystemXValidation`), options (`ValidationOptions`), exception (`RequestValidationException`), and rule extensions (`MustBeValidGuid`, `MustBeGuidV7`, `MustBePositiveId`, `NotEmptyGuid`, `ValidStringId`, `ValidateToResult`).
- SystemX.Infrastructure.Serialization: new package providing System.Text.Json converters (`StronglyTypedIdJsonConverter<TId, TValue>`, `StronglyTypedIdJsonConverterFactory`), type converters (`StronglyTypedIdTypeConverter`, `StronglyTypedIdTypeDescriptor`), and DI extensions (`AddSystemXConverters`, `AddSystemXSerialization`).
- SystemX.SharedKernel: added Domain primitives (`IEntity<TId>`, `Entity<TId>`, `IAggregateRoot`, `AggregateRoot<TId>`, `IHasDomainEvents`, `IDomainEvent`, `DomainEvent`, `ValueObject`, `Enumeration<TEnum>`, `IBusinessRule`, `BusinessRuleValidationException`, `DomainException`).
- SystemX.SharedKernel: added Auditing contracts and base entities (`IAuditable`, `AuditableEntity<TId>`, `AuditableAggregateRoot<TId>`, `ISoftDeletable`).
- SystemX.SharedKernel: added MultiTenancy primitives (`ITenantScoped`).
- SystemX.SharedKernel: added functional Result pattern and pipeline extensions (`Result`, `Result<T>`, `Error`, `ErrorType`, `ResultExtensions` with `Map`, `Bind`, `Ensure`, `Match`).
- SystemX.SharedKernel: added defensive argument assertions (`Guard`).
- SystemX.SharedKernel: added testable time abstractions (`IClock`, `SystemClock`).
- Initial project scaffolding for SystemX NuGet library suite.
- Central Package Management and Directory.Build.props setup.
- Automated release and CI workflows with MinVer.

### Changed
- SystemX.SharedKernel: made `Error` a non-sealed record to support polymorphic error types such as `ValidationError` without breaking existing call sites.
