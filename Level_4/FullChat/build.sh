#!/bin/bash

# Script to build all Docker images
set -e

echo "🚀 Building Chat Server and Client Docker images..."

# Build server image
echo "📦 Building Chat Server image..."
docker build -t chat-server -f Dockerfile.server .

# Build client image
echo "📦 Building Chat Client image..."
docker build -t chat-client -f Dockerfile.client .

echo "✅ All images built successfully!"
echo ""
echo "Available images:"
docker images | grep chat-
