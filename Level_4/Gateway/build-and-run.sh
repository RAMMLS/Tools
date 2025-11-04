#!/bin/bash

echo "🚀 Building and running Gateway project..."

# Build images individually
echo "🔨 Building Gateway..."
docker build -t api-gateway -f Dockerfile.gateway .

echo "🔨 Building Service A..."
docker build -t service-a -f Dockerfile.service-a .

echo "🔨 Building Service B..."
docker build -t service-b -f Dockerfile.service-b .

# Create network
echo "🌐 Creating network..."
docker network create gateway-network 2>/dev/null || echo "Network already exists"

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker stop api-gateway service-a service-b 2>/dev/null || true
docker rm api-gateway service-a service-b 2>/dev/null || true

# Run containers
echo "🆙 Starting containers..."
docker run -d --name service-a --network gateway-network service-a
docker run -d --name service-b --network gateway-network service-b
docker run -d --name api-gateway --network gateway-network -p 5000:5000 api-gateway

echo "✅ All services started!"
echo ""
echo "📊 Services status:"
echo "Gateway:   http://localhost:5000"
echo "Service A: http://localhost:5001"
echo "Service B: http://localhost:5002"
echo ""
echo "🔍 Check logs: docker logs api-gateway"
