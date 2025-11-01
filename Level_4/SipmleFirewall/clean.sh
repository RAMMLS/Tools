#!/bin/bash
# cleanup-and-build.sh

echo "🧹 Cleaning up previous builds..."

# Останавливаем и удаляем контейнеры
docker-compose down 2>/dev/null || true

# Удаляем образ
docker rmi simple-firewall 2>/dev/null || true

# Удаляем конфликтующие файлы
rm -f MinialProgram.cs MinimalProgram.cs AbsoluteMinimal.cs 2>/dev/null || true

# Очищаем билд-кэш
docker system prune -f

echo "🔨 Building new image..."
docker build -t simple-firewall .

echo "✅ Build completed successfully!"
echo ""
echo "To run the firewall:"
echo "  make compose-up"
echo "  or"
echo "  docker run -p 8080:8080 simple-firewall"
