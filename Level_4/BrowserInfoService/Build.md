# Сборка образа: 
docker build -t browser-info-service .

# Запуск контейнера: 
docker run -d -p 8080:80 --name browser-info-container browser-info-service

# Остановить контейнер: 
docker rm -f browser-info-container

