# Сборка образа: 
docker build -t browser-info-service .

# Запуск контейнера: 
docker run -d -p 8080:80 --name browser-info-container browser-info-service

# Остановить контейнер: 
[1] docker stop browser-info-container
[2] docker rm -f browser-info-container

# Перезапуск контейнера: 
docker restart browser-info-container

# Проверить контейнер: 
docker ps

# Посмотреть информацию с браузера:
curl http://localhost:8080/browserinfo
