#!/usr/bin/env bash

# This script is used to publish the YTBDLSharp project for Linux and Windows.

# Stop on error
set -e

# Variables
PROJECT="YTBDLSharp.csproj"
# Can be overridden by passing a version as the first argument, defaults to "dev"
VERSION="${1:-dev}"
FRAMEWORK="net10.0"
OUTPUT_DIR="publish"

echo "======================================"
echo "       YTBDLSharp Publisher"
echo "======================================"
echo "Version: $VERSION"
echo

# Clean previous release
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

echo "  Publishing Linux x64..."

dotnet publish "$PROJECT" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -o "$OUTPUT_DIR/linux-x64"

echo "  Publishing Windows x64..."

dotnet publish "$PROJECT" \
    -c Release \
    -r win-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -o "$OUTPUT_DIR/win-x64"

echo "  Creating archives..."

# Linux
tar -czf \
    "$OUTPUT_DIR/YTBDLSharp-v${VERSION}-linux-x64.tar.gz" \
    -C "$OUTPUT_DIR/linux-x64" \
    YTBDLSharp

# Windows
(
    cd "$OUTPUT_DIR/win-x64"
    zip -q "../YTBDLSharp-v${VERSION}-win-x64.zip" YTBDLSharp.exe
)

echo "  Cleaning temporary publish directories..."

rm -rf "$OUTPUT_DIR/linux-x64"
rm -rf "$OUTPUT_DIR/win-x64"

echo
echo "======================================"
echo "           Publish complete!"
echo "======================================"
echo
echo "Release files:"
ls -lh "$OUTPUT_DIR"