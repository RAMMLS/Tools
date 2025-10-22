#!/bin/bash

echo "=== Trying different Dockerfile configurations ==="

# Попробуем сначала официальный образ Mono
echo "Attempt 1: Using official Mono image..."
cat >Dockerfile.mono <<'EOF'
FROM mono:6.12.0.182
RUN apt-get update && apt-get install -y --no-install-recommends \
    x11-apps \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY MouseTracker.cs /app/
RUN mcs -r:System.Windows.Forms -r:System.Drawing -r:System.Data MouseTracker.cs
ENV DISPLAY=:0
CMD ["mono", "MouseTracker.exe"]
EOF

if docker build -t mouse-tracker -f Dockerfile.mono .; then
  echo "=== Build successful with official Mono image! ==="
  rm -f Dockerfile.mono
else
  echo "=== Attempt 1 failed, trying Ubuntu 20.04 ==="

  # Попробуем Ubuntu 20.04
  cat >Dockerfile.focal <<'EOF'
FROM ubuntu:20.04
ENV DEBIAN_FRONTEND=noninteractive
RUN apt-get update && apt-get install -y --no-install-recommends \
    mono-runtime \
    mono-mcs \
    libmono-system-windows-forms4.0-cil \
    libmono-system-drawing4.0-cil \
    x11-apps \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY MouseTracker.cs /app/
RUN mcs -r:System.Windows.Forms -r:System.Drawing -r:System.Data MouseTracker.cs
ENV DISPLAY=:0
CMD ["mono", "MouseTracker.exe"]
EOF

  if docker build -t mouse-tracker -f Dockerfile.focal .; then
    echo "=== Build successful with Ubuntu 20.04! ==="
    rm -f Dockerfile.focal
  else
    echo "=== All build attempts failed ==="
    rm -f Dockerfile.mono Dockerfile.focal
    exit 1
  fi
fi

echo "=== Setting up X11 permissions ==="
xhost +local:docker

echo "=== Starting container ==="
docker run -it --rm \
  -e DISPLAY=$DISPLAY \
  -v /tmp/.X11-unix:/tmp/.X11-unix \
  mouse-tracker
