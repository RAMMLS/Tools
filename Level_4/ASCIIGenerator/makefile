.PHONY: build run test clean push

# Переменные
IMAGE_NAME = ascii-name-generator
TAG = latest
CONTAINER_NAME = ascii-generator

# Сборка образа
build:
	docker build -t $(IMAGE_NAME):$(TAG) .

# Запуск контейнера
run:
	docker run -it --rm \
		--name $(CONTAINER_NAME) \
		-v $(PWD)/output:/app/output \
		$(IMAGE_NAME):$(TAG)

# Запуск в фоновом режиме
run-detached:
	docker run -d \
		--name $(CONTAINER_NAME) \
		-v $(PWD)/output:/app/output \
		$(IMAGE_NAME):$(TAG)

# Запуск с docker-compose
compose-up:
	docker-compose up -d

compose-down:
	docker-compose down

# Просмотр логов
logs:
	docker logs -f $(CONTAINER_NAME)

# Вход в контейнер
exec:
	docker exec -it $(CONTAINER_NAME) /bin/bash

# Остановка и очистка
stop:
	docker stop $(CONTAINER_NAME)

clean:
	docker rm $(CONTAINER_NAME) || true
	docker rmi $(IMAGE_NAME):$(TAG) || true

# Полная очистка системы Docker
clean-all:
	docker system prune -f

# Тестирование
test:
	docker run --rm $(IMAGE_NAME):$(TAG) echo "Hello World"

# Публикация в registry (пример)
push:
	docker tag $(IMAGE_NAME):$(TAG) your-registry/$(IMAGE_NAME):$(TAG)
	docker push your-registry/$(IMAGE_NAME):$(TAG)

help:
	@echo "Available commands:"
	@echo "  build        - Build Docker image"
	@echo "  run          - Run container interactively"
	@echo "  run-detached - Run container in background"
	@echo "  compose-up   - Start with docker-compose"
	@echo "  compose-down - Stop docker-compose"
	@echo "  logs         - View container logs"
	@echo "  exec         - Enter container"
	@echo "  stop         - Stop container"
	@echo "  clean        - Remove container and image"
	@echo "  clean-all    - Full Docker system cleanup"
	@echo "  test         - Test container"
	@echo "  push         - Push to registry"
