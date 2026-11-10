#!/usr/bin/env bash
set -euo pipefail

if [ $# -lt 1 ]; then
    echo "Usage: $0 <version>"
    echo "Example: $0 1.0.0"
    exit 1
fi

VERSION="$1"
# Strip leading 'v' if provided
VERSION="${VERSION#v}"

# SemVer validation regex
SEMVER_REGEX="^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)(-([0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?(\+([0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*))?$"

if ! [[ "$VERSION" =~ $SEMVER_REGEX ]]; then
    echo "Error: '$VERSION' is not a valid Semantic Version." >&2
    exit 1
fi

TAG="v${VERSION}"

if git rev-parse "$TAG" >/dev/null 2>&1; then
    echo "Error: Tag '$TAG' already exists locally or remotely." >&2
    exit 1
fi

echo "Creating annotated tag $TAG..."
git tag -a "$TAG" -m "Release $TAG"

echo "Pushing tag $TAG to origin..."
git push origin "$TAG"

echo "Successfully tagged and pushed $TAG."
