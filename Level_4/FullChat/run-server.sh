#!/bin/bash

# Script to run chat server
set -e

echo "🚀 Starting Chat Server..."

# Check if image exists, build if not
if ! docker images | grep -q "chat-server"; then
  echo "📦 Building Chat Server image..."
  docker build -t chat-server -f Dockerfile.server .
fi

# Run server container
docker run -it --rm \
  --name chat-server \
  -p 5000:5000 \
  -p 5001:5000 \
  -v chat-data:/app/files \
  -v $(pwd)/logs:/app/logs \
  -e ASPNETCORE_ENVIRONMENT=Production \
  chat-server
