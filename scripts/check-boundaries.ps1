# Fails when EF Core leaks outside the persistence projects or project references break AGENTS.md rules.
$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location (Join-Path $scriptDir "..")

$status = 0
$allowedRegex = '^src[\\/]SystemX\.Infrastructure\.Persistence\.(EfCore|SqlServer|Postgresql)[\\/]'

function Fail($message) {
    Write-Host "BOUNDARY VIOLATION: $message" -ForegroundColor Red
    $script:status = 1
}

# 1) EF Core namespaces only in persistence projects
$csFiles = Get-ChildItem -Path src -Filter "*.cs" -Recurse | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
foreach ($file in $csFiles) {
    $relPath = [System.IO.Path]::GetRelativePath((Get-Location).Path, $file.FullName)
    if (Select-String -Path $file.FullName -Pattern 'Microsoft\.EntityFrameworkCore' -Quiet) {
        if ($relPath -notmatch $allowedRegex) {
            Fail "EF Core namespace used in $relPath"
        }
    }
}

# 2) EF Core packages only in persistence projects
$projFiles = Get-ChildItem -Path src -Include "*.csproj","*.props","*.targets" -Recurse | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
foreach ($file in $projFiles) {
    $relPath = [System.IO.Path]::GetRelativePath((Get-Location).Path, $file.FullName)
    if (Select-String -Path $file.FullName -Pattern 'Include="Microsoft\.EntityFrameworkCore' -Quiet) {
        if ($relPath -notmatch $allowedRegex) {
            Fail "EF Core package referenced in $relPath"
        }
    }
}

# 3) No IQueryable in abstractions and contracts
$checkDirs = @(
    "src/SystemX.Application.Abstractions",
    "src/SystemX.Contracts",
    "src/SystemX.Contracts.Validation",
    "src/SystemX.SharedKernel"
)
foreach ($dir in $checkDirs) {
    if (Test-Path $dir) {
        $files = Get-ChildItem -Path $dir -Filter "*.cs" -Recurse | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
        foreach ($file in $files) {
            $relPath = [System.IO.Path]::GetRelativePath((Get-Location).Path, $file.FullName)
            if (Select-String -Path $file.FullName -Pattern '\bIQueryable\b' -Quiet) {
                Fail "IQueryable used in $relPath (repositories must not expose IQueryable)"
            }
        }
    }
}

# 4) Project reference rules (AGENTS.md section 2)
function Get-ProjectReferences($csprojPath) {
    $refs = @()
    if (Test-Path $csprojPath) {
        $content = Get-Content $csprojPath
        foreach ($line in $content) {
            if ($line -match 'ProjectReference Include="([^"]+)"') {
                $refFile = [System.IO.Path]::GetFileNameWithoutExtension($Matches[1])
                $refs += $refFile
            }
        }
    }
    return $refs
}

foreach ($p in @("SystemX.SharedKernel", "SystemX.Contracts")) {
    $refs = Get-ProjectReferences "src/$p/$p.csproj"
    if ($refs.Count -gt 0) {
        Fail "$p must not reference other projects"
    }
}

$appAbsCsproj = "src/SystemX.Application.Abstractions/SystemX.Application.Abstractions.csproj"
if (Test-Path $appAbsCsproj) {
    $refs = Get-ProjectReferences $appAbsCsproj
    $bad = $refs | Where-Object { $_ -ne "SystemX.SharedKernel" }
    if ($bad) {
        Fail "Application.Abstractions may reference only SharedKernel (found: $($bad -join ', '))"
    }
}

$infraProjects = Get-ChildItem -Path "src" -Filter "*.csproj" -Recurse | Where-Object { $_.FullName -match 'src[\\/]SystemX\.Infrastructure\.[^\\/]+[\\/]' }
foreach ($proj in $infraProjects) {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($proj.FullName)
    if ($name -eq "SystemX.Infrastructure") { continue }
    $refs = Get-ProjectReferences $proj.FullName
    foreach ($ref in $refs) {
        if ($ref -like "SystemX.Infrastructure.*") {
            $allowed = $false
            if ($name -eq "SystemX.Infrastructure.Persistence.SqlServer" -and $ref -eq "SystemX.Infrastructure.Persistence.EfCore") { $allowed = $true }
            if ($name -eq "SystemX.Infrastructure.Persistence.Postgresql" -and $ref -eq "SystemX.Infrastructure.Persistence.EfCore") { $allowed = $true }
            if (-not $allowed) {
                Fail "$name must not reference $ref"
            }
        }
    }
}

# 5) Hangfire and Quartz only in the BackgroundJobs project
$bgFiles = Get-ChildItem -Path src -Include "*.cs","*.csproj" -Recurse | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
foreach ($file in $bgFiles) {
    $relPath = [System.IO.Path]::GetRelativePath((Get-Location).Path, $file.FullName)
    if (Select-String -Path $file.FullName -Pattern '(using|Include="|namespace )(Hangfire|Quartz)\b' -Quiet) {
        if ($relPath -notmatch '^src[\\/]SystemX\.Infrastructure\.BackgroundJobs[\\/]') {
            Fail "Hangfire/Quartz used in $relPath"
        }
    }
}

if ($status -eq 0) {
    Write-Host "Boundary check passed." -ForegroundColor Green
} else {
    Write-Host "Boundary check FAILED." -ForegroundColor Red
}
exit $status
