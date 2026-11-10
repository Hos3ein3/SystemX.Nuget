# AGENTS.md - SystemX.Nuget rules

These rules apply to every agent and contributor working in this repository.
Read this file first. Check it before adding or changing any reference, public API or documentation.

## 1. Layers and purpose

Every project has ONE purpose. If new code does not fit the purpose of a project, put it in the right project or create a new package. Do not stretch a project.

### 1.1 Core layers (no infrastructure, minimal dependencies)

| Project | Purpose | Contains | Must NOT contain |
|---|---|---|---|
| `SystemX.SharedKernel` | Domain and application building blocks shared by all apps. | Entity, AggregateRoot, ValueObject, Enumeration, domain events, business rules, Result and Error, Guard, IClock, strongly-typed IDs, specifications, pagination types, multi-tenancy model/info types, concurrency markers, entity markers, translations, audit/soft-delete markers. | Third-party packages, EF Core, ASP.NET, JSON, logging, any I/O. |
| `SystemX.Contracts` | The public data shapes exchanged between services and clients. | Request/response DTOs, `PagedRequest`/`PagedResponse<T>`, integration events, error codes. Plain records. | Logic, domain types, interfaces for behavior, any reference to another SystemX project or third-party package. |
| `SystemX.Application.Abstractions` | The interfaces the application layer codes against. | `ICommand`, `IQuery`, handler interfaces, dispatcher interfaces, `IUnitOfWork`, `IRepository<T>`, `IReadRepository<T>`, `ICurrentUser`, `ITenantContext`, `ITenantContextAccessor`, `ITenantStore`, `ITenantResolutionStrategy`, `ITenantConnectionResolver`, `ITenantSchemaProvider`, `ITenantProvisioner`, `ICurrentCulture`, `ICacheService`, `IEventBus`. | Implementations, third-party packages, anything from Infrastructure. |

### 1.2 Validation and serialization

| Project | Purpose |
|---|---|
| `SystemX.Contracts.Validation` | FluentValidation validators for the shared DTOs in `SystemX.Contracts`. Lets clients and services validate the same DTOs without making `Contracts` depend on FluentValidation. |
| `SystemX.Infrastructure.Validation` | Runs FluentValidation on commands and queries through a handler decorator. Turns failures into `Result` with `ValidationError` (or throws `RequestValidationException`). Provides reusable rules for IDs. |
| `SystemX.Infrastructure.Serialization` | `System.Text.Json` support: converters and type converters for strongly-typed IDs and other SystemX types. |

### 1.3 Application and API infrastructure

| Project | Purpose |
|---|---|
| `SystemX.Infrastructure.Cqrs` | Command/query dispatchers (no MediatR), handler registration and handler decorators (logging, transaction). |
| `SystemX.Infrastructure.Messaging` | Event bus, integration-event publishing and consuming, outbox/inbox. |
| `SystemX.Infrastructure.Web` | ASP.NET Core pipeline setup: middleware, ProblemDetails, API versioning, Swagger, Result-to-HTTP mapping, JSON option wiring. |
| `SystemX.Infrastructure.Cors` | CORS policy setup with options. |
| `SystemX.Infrastructure.RateLimiter` | Rate-limiting policies (ASP.NET Core rate limiting) with options. |
| `SystemX.Infrastructure.Localization` | Culture resolution, resource-based localization, `ICurrentCulture` implementation. |
| `SystemX.Infrastructure.MultiTenancy` | Tenant resolution (header, claim, host), `ITenantContext` implementation, tenant query filters. |

### 1.4 Data

| Project | Purpose |
|---|---|
| `SystemX.Infrastructure.Persistence.EfCore` | Provider-neutral EF Core base: `UnitOfWork`, generic repositories, specification evaluator, interceptors (audit, soft delete, tenant, domain-event dispatch), strongly-typed ID value converters. |
| `SystemX.Infrastructure.Persistence.SqlServer` | SQL Server provider setup on top of `EfCore` (connection, options, SQL Server specifics only). |
| `SystemX.Infrastructure.Persistence.Postgresql` | PostgreSQL provider setup on top of `EfCore` (Npgsql, PostgreSQL specifics only). |
| `SystemX.Infrastructure.Persistence.Mongo` | MongoDB repositories, serializers and conventions. Independent from EF Core. |
| `SystemX.Infrastructure.Caching` | Cache abstraction implementations (memory, Redis), cache decorators and key conventions. |

### 1.5 Observability and reliability

| Project | Purpose |
|---|---|
| `SystemX.Infrastructure.Logging` | Structured logging setup (for example Serilog), enrichers (correlation id, tenant, user), log options. |
| `SystemX.Infrastructure.Observability` | OpenTelemetry traces and metrics, exporters, instrumentation, naming conventions. |
| `SystemX.Infrastructure.HealthChecks` | Health check registration and endpoints (liveness, readiness), checks for the SystemX data and cache providers. |
| `SystemX.Infrastructure.Resilience` | Retry, timeout, circuit breaker and bulkhead policies (Polly / Microsoft.Extensions.Resilience) with options. |

### 1.6 Meta-package

| Project | Purpose |
|---|---|
| `SystemX.Infrastructure` | Convenience package for the common Infrastructure packages. Contains NO source code, only references. It never includes `Contracts.Validation`, a database provider (`SqlServer`, `Postgresql`, `Mongo`) or other optional packages. |

## 2. Dependency rules (hard rules)

1. `SystemX.SharedKernel` MUST NOT reference any other SystemX project or third-party package.
2. `SystemX.Contracts` MUST NOT reference any other SystemX project or third-party package.
3. `SystemX.Application.Abstractions` MAY reference only `SystemX.SharedKernel`.
4. `SystemX.Contracts.Validation` MAY reference only `SystemX.Contracts` and FluentValidation.
5. `SystemX.Infrastructure.Serialization` MAY reference only `SystemX.SharedKernel` (plus the .NET base class library).
6. `SystemX.Infrastructure.Validation` MAY reference `SharedKernel`, `Contracts`, `Application.Abstractions` and FluentValidation. It MUST NOT reference `Infrastructure.Cqrs`.
7. `SystemX.Infrastructure.Persistence.SqlServer` and `SystemX.Infrastructure.Persistence.Postgresql` MUST reference `SystemX.Infrastructure.Persistence.EfCore`.
8. `SqlServer` and `Postgresql` MUST NOT reference each other.
9. `SystemX.Infrastructure.Persistence.Mongo` and `SystemX.Infrastructure.Persistence.EfCore` MUST NOT reference each other.
10. A `SystemX.Infrastructure.*` package MUST NOT reference another `SystemX.Infrastructure.*` package, with only these exceptions:
    - A provider package referencing its base package (`SqlServer` -> `EfCore`, `Postgresql` -> `EfCore`).
    - The `SystemX.Infrastructure` meta-package referencing other Infrastructure packages.
11. Infrastructure packages MAY reference `SharedKernel`, `Contracts` and `Application.Abstractions`.
12. `SharedKernel`, `Contracts` and `Application.Abstractions` MUST NOT reference any `SystemX.Infrastructure.*` package or `SystemX.Contracts.Validation`.
13. The meta-package `SystemX.Infrastructure` MUST contain no source code.
14. A shared need between two Infrastructure packages is solved by moving the shared abstraction down into `Application.Abstractions` or `SharedKernel`, never by adding a reference between them.

### Allowed reference matrix

`X` = allowed. Blank = forbidden.

| From \ To | SharedKernel | Contracts | App.Abstractions | Infra.EfCore | Other Infra.* | Contracts.Validation |
|---|---|---|---|---|---|---|
| SharedKernel | | | | | | |
| Contracts | | | | | | |
| App.Abstractions | X | | | | | |
| Contracts.Validation | | X | | | | |
| Infra.Serialization | X | | | | | |
| Infra.Validation | X | X | X | | | |
| Infra.* (all other) | X | X | X | | | |
| Infra.Persistence.SqlServer / Postgresql | X | X | X | X | | |
| Infra.Persistence.Mongo | X | X | X | | | |
| SystemX.Infrastructure (meta) | | | | X | X | |

## 3. Documentation rules (MANDATORY)

A change is NOT complete until the documentation is updated. Treat missing documentation as a failing build.

### 3.1 CHANGELOG.md
- The file `CHANGELOG.md` MUST exist at the repo root. If it is missing, create it first.
- Format: [Keep a Changelog](https://keepachangelog.com) with [Semantic Versioning](https://semver.org).
- Every pull request or commit that changes behavior, public API, dependencies or packaging MUST add an entry under `## [Unreleased]`.
- Allowed sections: `Added`, `Changed`, `Deprecated`, `Removed`, `Fixed`, `Security`.
- Each entry MUST name the affected package, for example: `- SystemX.Infrastructure.Caching: added Redis provider.`
- Breaking changes MUST be marked with `**BREAKING**` at the start of the entry.
- On release, move `Unreleased` entries into a new section `## [x.y.z] - YYYY-MM-DD` and update the compare links at the bottom.

### 3.2 Per-feature documentation
- Every public piece of code (each public type or feature group, for example `Entity`, `Result`, `ICommandDispatcher`, `AddSystemXCaching`) MUST have a document.
- Location: `docs/<PackageName>/<FeatureName>.md`. Example: `docs/SystemX.SharedKernel/Result.md`.
- Each package folder MUST also have an `index` section in the package README that states the package purpose (copy it from section 1 of this file).
- Each document MUST use this template:

```markdown
# <FeatureName>

**Package:** `SystemX.<Package>`
**Namespace:** `SystemX.<Namespace>`

## What it is
One or two sentences.

## When to use it
- Use it when ...
- Do NOT use it when ...

## Installation
`dotnet add package SystemX.<Package>`

## How to use it
Step-by-step usage, including registration (DI) and options if they exist.

## Sample code
Complete, compilable C# sample (include usings). Add a second sample for a realistic scenario.

## Configuration (if any)
Table of options, defaults and descriptions.

## Related
Links to related docs.
```

- Samples MUST compile against the current API. If the API changes, update the sample in the same change.
- Public types MUST have XML documentation comments (`///`), which feed IntelliSense.

### 3.3 README links
- Every package has `src/<PackageName>/README.md`. It MUST start with the package purpose and contain a "Documentation" section linking to each document of that package in `docs/<PackageName>/`.
- The root `README.md` MUST contain a "Documentation" section with one sub-list per package, linking to every document in `docs/`.
- When adding, renaming or deleting a document, update both README files in the same change.

### 3.4 Root README required sections
The root `README.md` MUST always contain these sections, in this order:
1. Overview and package table (with the purpose of each package)
2. Dependency diagram (Mermaid)
3. Documentation (links to all docs, grouped by package)
4. How to publish packages to NuGet
5. How to add the packages to your projects
6. Contributing, Changelog and License links

Section 4 (publish) MUST explain:
- One-time setup: reserve the `SystemX.` ID prefix on nuget.org, create an API key (scope: Push, glob `SystemX.*`), and save it as the GitHub secret `NUGET_API_KEY`.
- Release flow: run the `bump-version` workflow (patch/minor/major/custom) or push a tag `vX.Y.Z`; the `release` workflow builds, tests, packs and pushes `.nupkg` and `.snupkg`.
- Manual publish: `dotnet pack -c Release -o artifacts` then `dotnet nuget push "artifacts/*.nupkg" --source https://api.nuget.org/v3/index.json --api-key <KEY>`.
- Rules: published versions cannot be deleted (only unlisted), so use prerelease versions (`-preview.N`) for tests; update `CHANGELOG.md` before tagging.

Section 5 (consume) MUST explain, with examples:
- `dotnet add package SystemX.SharedKernel` (and prerelease: `--prerelease`).
- Using Central Package Management (`<PackageVersion Include="SystemX.SharedKernel" Version="1.0.0" />`).
- Installing the meta-package `SystemX.Infrastructure` versus single packages.
- A minimal `Program.cs` registration example.
- Using a private feed (GitHub Packages or Azure Artifacts) through `nuget.config`, and a local feed for testing (`dotnet nuget add source ./artifacts --name local`).
- Updating: `dotnet list package --outdated`.

## 4. Agent behavior

- Before adding code, decide which project owns it using the purpose table in section 1. If none fits, ask the maintainer or propose a new package (and update this file first).
- If a task needs a forbidden reference, STOP. Move the shared code down into `SharedKernel`, `Contracts` or `Application.Abstractions`, or ask the maintainer.
- Never add a reference only to make a build error disappear.
- Do not add third-party packages to `SharedKernel`, `Contracts` or `Application.Abstractions` without maintainer approval.
- Keep package versions in `Directory.Packages.props` (Central Package Management).
- A new Infrastructure concern = a new package. Do not extend an unrelated package.
- When a package is added, renamed or removed, update section 1, section 2, the matrix, the meta-package, the `.slnx`, the README package table and the Mermaid diagram in the same change.
- Before finishing any task, run the Definition of Done checklist below.

## 5. Definition of Done checklist

- [ ] Build and tests pass in Release.
- [ ] Architecture tests pass (all rules in section 2).
- [ ] `CHANGELOG.md` has an entry under `Unreleased`.
- [ ] A doc file exists in `docs/<Package>/` for each new or changed public feature, using the template.
- [ ] Doc samples compile.
- [ ] Package `README.md` (starting with the purpose) and root `README.md` links are updated.
- [ ] Public types have XML comments.
- [ ] The code lives in the project whose purpose matches it (section 1).

## 6. Enforcement

- Architecture tests (NetArchTest or ArchUnitNET) in `tests/SystemX.ArchitectureTests` encode section 2.
- CI MUST fail when an architecture test fails.
- CI SHOULD include a docs check: every `src/<Package>/` has a `docs/<Package>/` folder, `CHANGELOG.md` is modified in pull requests that change `src/`, and markdown links are not broken.
- When a rule changes, update this file and the checks in the same pull request.
