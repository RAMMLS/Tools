#!/bin/bash

echo "=== Checking files ==="
ls -la MouseTracker.cs Dockerfile

echo "=== Testing Mono compilation locally ==="
if command -v mcs >/dev/null 2>&1; then
  mcs -r:System.Windows.Forms -r:System.Drawing -r:System.Data MouseTracker.cs
  if [ $? -eq 0 ]; then
    echo "Local compilation successful"
  else
    echo "Local compilation failed"
  fi
else
  echo "Mono compiler not found locally, will use Docker"
fi

echo "=== Building in Docker ==="
docker build -t mouse-tracker .
