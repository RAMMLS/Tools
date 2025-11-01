#!/bin/bash

# Script to build Simple Firewall Docker image
set -e

echo "🔥 Building Simple Firewall Docker image..."

# Build the image
docker build -t simple-firewall .

echo "✅ Image built successfully!"
echo ""
echo "Available images:"
docker images | grep simple-firewall
echo ""
echo "To run the firewall:"
echo "  make run-privileged"
echo "  or"
echo "  make compose-up"
