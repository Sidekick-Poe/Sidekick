#!/usr/bin/env bash
set -e

NUGET_ROOT="$1"
OUT_DIR="$2"

BASE="$NUGET_ROOT/microsoft.aspnetcore.app.internal.assets"

# Find version folders
versions=$(find "$BASE" -maxdepth 1 -type d -name "10.*" | sort -V)

if [ -z "$versions" ]; then
  echo "No version folders found under $BASE"
  exit 1
fi

# Pick the last (highest) version
LATEST=$(printf "%s\n" $versions | tail -n 1)
echo "Using Blazor framework from: $LATEST"

SRC="$LATEST/_framework"
DST="$OUT_DIR/wwwroot/_framework"

mkdir -p "$DST"
cp -R "$SRC/"* "$DST/"
