#!/usr/bin/env bash
# Fails when EF Core leaks outside the persistence projects or project references break AGENTS.md rules.
set -euo pipefail
cd "$(dirname "$0")/.."

status=0
allowed='^src/SystemX\.Infrastructure\.Persistence\.(EfCore|SqlServer|Postgresql)/'
fail() { echo "BOUNDARY VIOLATION: $1"; status=1; }

# 1) EF Core namespaces only in persistence projects
while IFS= read -r file; do
  [[ "$file" =~ $allowed ]] || fail "EF Core namespace used in $file"
done < <(grep -rlE --include='*.cs' 'Microsoft\.EntityFrameworkCore' src 2>/dev/null | grep -vE '/(obj|bin)/' || true)

# 2) EF Core packages only in persistence projects
while IFS= read -r file; do
  [[ "$file" =~ $allowed ]] || fail "EF Core package referenced in $file"
done < <(grep -rlE --include='*.csproj' --include='*.props' --include='*.targets' 'Include="Microsoft\.EntityFrameworkCore' src 2>/dev/null || true)

# 3) No IQueryable in abstractions and contracts
for dir in src/SystemX.Application.Abstractions src/SystemX.Contracts src/SystemX.Contracts.Validation src/SystemX.SharedKernel; do
  [ -d "$dir" ] || continue
  while IFS= read -r file; do
    fail "IQueryable used in $file (repositories must not expose IQueryable)"
  done < <(grep -rlE --include='*.cs' '\bIQueryable\b' "$dir" 2>/dev/null | grep -vE '/(obj|bin)/' || true)
done

# 4) Project reference rules (AGENTS.md section 2)
refs() { grep -oE 'ProjectReference Include="[^"]+"' "$1" 2>/dev/null | sed -E 's/.*\\|.*\///; s/"//; s/\.csproj//' || true; }

for p in SystemX.SharedKernel SystemX.Contracts; do
  [ -n "$(refs src/$p/$p.csproj)" ] && fail "$p must not reference other projects"
done
if [ -f src/SystemX.Application.Abstractions/SystemX.Application.Abstractions.csproj ]; then
  bad=$(refs src/SystemX.Application.Abstractions/SystemX.Application.Abstractions.csproj | grep -vx 'SystemX.SharedKernel' || true)
  [ -n "$bad" ] && fail "Application.Abstractions may reference only SharedKernel (found: $bad)"
fi
for csproj in src/SystemX.Infrastructure.*/*.csproj; do
  name=$(basename "$csproj" .csproj)
  [ "$name" = "SystemX.Infrastructure" ] && continue
  while IFS= read -r ref; do
    [[ "$ref" == SystemX.Infrastructure.* ]] || continue
    case "$name->$ref" in
      "SystemX.Infrastructure.Persistence.SqlServer->SystemX.Infrastructure.Persistence.EfCore") ;;
      "SystemX.Infrastructure.Persistence.Postgresql->SystemX.Infrastructure.Persistence.EfCore") ;;
      *) fail "$name must not reference $ref" ;;
    esac
  done < <(refs "$csproj")
done

# 5) Hangfire and Quartz only in the BackgroundJobs project
while IFS= read -r file; do
  [[ "$file" =~ ^src/SystemX\.Infrastructure\.BackgroundJobs/ ]] || fail "Hangfire/Quartz used in $file"
done < <(grep -rlE --include='*.cs' --include='*.csproj' '(using|Include="|namespace )[[:space:]]*(Hangfire|Quartz)([[:space:]]|;|\.|\"|$)' src 2>/dev/null | grep -vE '/(obj|bin)/' || true)

[ "$status" -eq 0 ] && echo "Boundary check passed." || echo "Boundary check FAILED."
exit "$status"
