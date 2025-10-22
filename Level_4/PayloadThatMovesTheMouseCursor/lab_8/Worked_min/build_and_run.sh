#!/bin/bash

echo "=== Step 1: Clean up previous builds ==="
docker rmi mouse-tracker 2>/dev/null || true

echo "=== Step 2: Building docker image ==="
if docker build -t mouse-tracker .; then
  echo "=== Build successful! ==="

  echo "=== Step 3: Setting up X11 permissions ==="
  xhost +local:docker

  echo "=== Step 4: Starting container ==="
  docker run -it --rm \
    -e DISPLAY=$DISPLAY \
    -v /tmp/.X11-unix:/tmp/.X11-unix \
    mouse-tracker
else
  echo "=== Build failed! Checking logs... ==="
  exit 1
fi
