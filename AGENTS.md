# AGENTS.md - SystemX.Nuget rules

These rules apply to every agent and contributor working in this repository.
Read this file first. Check it before adding or changing any reference, public API or documentation.

## 1. Layers

| Project | Role |
|---|---|
| `SystemX.SharedKernel` | Domain primitives (Entity, AggregateRoot, ValueObject, Result) |
| `SystemX.Contracts` | DTOs, requests/responses, integration events |
| `SystemX.Application.Abstractions` | CQRS, UnitOfWork, repository and context interfaces |
| `SystemX.Infrastructure.*` | One package per technical concern |
| `SystemX.Infrastructure` | Meta-package (no code, references only) |

## 2. Dependency rules (hard rules)

1. `SystemX.SharedKernel` MUST NOT reference any other SystemX project.
2. `SystemX.Contracts` MUST NOT reference any other SystemX project.
3. `SystemX.Application.Abstractions` MAY reference only `SystemX.SharedKernel`.
4. `SystemX.Infrastructure.Persistence.SqlServer` and `SystemX.Infrastructure.Persistence.Postgresql` MUST reference `SystemX.Infrastructure.Persistence.EfCore`.
5. `SqlServer` and `Postgresql` MUST NOT reference each other.
6. `SystemX.Infrastructure.Persistence.Mongo` and `SystemX.Infrastructure.Persistence.EfCore` MUST NOT reference each other.
7. A `SystemX.Infrastructure.*` package MUST NOT reference another `SystemX.Infrastructure.*` package, with only these exceptions:
   - A provider package referencing its base package (`SqlServer` -> `EfCore`, `Postgresql` -> `EfCore`).
   - The `SystemX.Infrastructure` meta-package referencing other Infrastructure packages.
8. Infrastructure packages MAY reference `SharedKernel`, `Contracts` and `Application.Abstractions`.
9. `SharedKernel`, `Contracts` and `Application.Abstractions` MUST NOT reference any `SystemX.Infrastructure.*` package.
10. The meta-package `SystemX.Infrastructure` MUST contain no source code.

### Allowed reference matrix

`X` = allowed. Blank = forbidden.

| From \ To | SharedKernel | Contracts | App.Abstractions | Infra.EfCore | Infra.Other |
|---|---|---|---|---|---|
| SharedKernel | | | | | |
| Contracts | | | | | |
| App.Abstractions | X | | | | |
| Infra.* (general) | X | X | X | | |
| Infra.Persistence.SqlServer / Postgresql | X | X | X | X | |
| Infra.Persistence.Mongo | X | X | X | | |
| SystemX.Infrastructure (meta) | | | | X | X |

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
- Every package has `src/<PackageName>/README.md`. It MUST contain a "Documentation" section linking to each document of that package in `docs/<PackageName>/`.
- The root `README.md` MUST contain a "Documentation" section with one sub-list per package, linking to every document in `docs/`.
- When adding, renaming or deleting a document, update both README files in the same change.

### 3.4 Root README required sections
The root `README.md` MUST always contain these sections, in this order:
1. Overview and package table
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

- If a task needs a forbidden reference, STOP. Move the shared code down into `SharedKernel`, `Contracts` or `Application.Abstractions`, or ask the maintainer.
- Never add a reference only to make a build error disappear.
- Do not add third-party packages to `SharedKernel`, `Contracts` or `Application.Abstractions` without maintainer approval.
- Keep package versions in `Directory.Packages.props` (Central Package Management).
- A new Infrastructure concern = a new package. Do not extend an unrelated package.
- Before finishing any task, run the Definition of Done checklist below.

## 5. Definition of Done checklist

- [ ] Build and tests pass in Release.
- [ ] Architecture tests pass (all rules in section 2).
- [ ] `CHANGELOG.md` has an entry under `Unreleased`.
- [ ] A doc file exists in `docs/<Package>/` for each new or changed public feature, using the template.
- [ ] Doc samples compile.
- [ ] Package `README.md` and root `README.md` links are updated.
- [ ] Public types have XML comments.

## 6. Enforcement

- Architecture tests (NetArchTest or ArchUnitNET) in `tests/SystemX.ArchitectureTests` encode section 2.
- CI MUST fail when an architecture test fails.
- CI SHOULD include a docs check: every `src/<Package>/` has a `docs/<Package>/` folder, `CHANGELOG.md` is modified in pull requests that change `src/`, and markdown links are not broken.
- When a rule changes, update this file and the checks in the same pull request.
