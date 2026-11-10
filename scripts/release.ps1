param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

$cleanVersion = $Version.TrimStart('v')
$semVerRegex = '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(-([0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?(\+([0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?$'

if ($cleanVersion -notmatch $semVerRegex) {
    Write-Error "Error: '$Version' is not a valid Semantic Version."
    exit 1
}

$tag = "v$cleanVersion"

$existingTag = git tag -l $tag
if ($existingTag) {
    Write-Error "Error: Tag '$tag' already exists."
    exit 1
}

Write-Host "Creating annotated tag $tag..."
git tag -a $tag -m "Release $tag"

Write-Host "Pushing tag $tag to origin..."
git push origin $tag

Write-Host "Successfully tagged and pushed $tag."
