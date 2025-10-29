#!/bin/bash

echo "🔄 Restarting Chat Server..."

# Останавливаем и удаляем старый контейнер
docker rm -f chat-server 2>/dev/null || echo "No existing chat-server container"

# Создаем сеть если нужно
docker network create chat-net 2>/dev/null || echo "Network chat-net already exists"

# Запускаем новый контейнер
docker run -d -p 5000:5000 --name chat-server --network chat-net chat-server

echo "✅ Chat Server restarted successfully!"
echo "📋 Check logs: docker logs chat-server"
