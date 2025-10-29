#!/bin/bash

# Script to run chat client
set -e

SERVER_HOST=${1:-localhost}
SERVER_PORT=${2:-5000}
CLIENT_NAME=${3:-chat-client}

echo "🚀 Starting Chat Client connected to $SERVER_HOST:$SERVER_PORT..."

# Check if image exists, build if not
if ! docker images | grep -q "chat-client"; then
  echo "📦 Building Chat Client image..."
  docker build -t chat-client -f Dockerfile.client .
fi

# Run client container
docker run -it --rm \
  --name "$CLIENT_NAME" \
  -e SERVER_HOST="$SERVER_HOST" \
  -e SERVER_PORT="$SERVER_PORT" \
  -v "$(pwd)/downloads-$CLIENT_NAME":/app/downloads \
  chat-client
