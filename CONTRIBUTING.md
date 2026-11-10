# Contributing to SystemX

Thank you for contributing to SystemX!

## Branching Strategy
- `main`: Protected production branch.
- Feature/Bugfix branches: branch off `main` as `feature/<name>` or `fix/<name>`.

## Commit Style
We encourage conventional commit messages:
- `feat: <description>`
- `fix: <description>`
- `chore: <description>`
- `docs: <description>`

## How to Release
Releases are automated via GitHub Actions:
1. Versioning is driven by git tags (e.g. `v1.0.0`) using MinVer.
2. Run the `bump-version.yml` workflow via GitHub Actions or use `scripts/release.sh <version>`.
3. Creating and pushing a `v*` tag triggers the `release.yml` workflow to publish packages to NuGet.
