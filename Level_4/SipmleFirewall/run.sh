#!/bin/bash

# Script to run Simple Firewall
set -e

echo "🔥 Starting Simple Firewall..."

# Check if image exists
if ! docker images | grep -q "simple-firewall"; then
  echo "📦 Building image first..."
  ./build.sh
fi

# Run with privileged mode for packet capture
docker run -it --rm \
  --name simple-firewall \
  -p 8080:8080 \
  --cap-add=NET_ADMIN \
  --cap-add=NET_RAW \
  --privileged \
  -v $(pwd)/logs:/app/logs \
  -v $(pwd)/rules:/app/rules \
  simple-firewall
