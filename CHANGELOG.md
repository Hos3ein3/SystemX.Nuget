# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- SystemX.SharedKernel: added Domain primitives (`IEntity<TId>`, `Entity<TId>`, `IAggregateRoot`, `AggregateRoot<TId>`, `IHasDomainEvents`, `IDomainEvent`, `DomainEvent`, `ValueObject`, `Enumeration<TEnum>`, `IBusinessRule`, `BusinessRuleValidationException`, `DomainException`).
- SystemX.SharedKernel: added Auditing contracts and base entities (`IAuditable`, `AuditableEntity<TId>`, `AuditableAggregateRoot<TId>`, `ISoftDeletable`).
- SystemX.SharedKernel: added MultiTenancy primitives (`ITenantScoped`).
- SystemX.SharedKernel: added functional Result pattern and pipeline extensions (`Result`, `Result<T>`, `Error`, `ErrorType`, `ResultExtensions` with `Map`, `Bind`, `Ensure`, `Match`).
- SystemX.SharedKernel: added defensive argument assertions (`Guard`).
- SystemX.SharedKernel: added testable time abstractions (`IClock`, `SystemClock`).
- Initial project scaffolding for SystemX NuGet library suite.
- Central Package Management and Directory.Build.props setup.
- Automated release and CI workflows with MinVer.
