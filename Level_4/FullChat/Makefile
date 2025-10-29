.PHONY: build build-server build-client run run-server run-client compose-up compose-down clean help

# Variables
SERVER_IMAGE = chat-server
CLIENT_IMAGE = chat-client
COMPOSE_FILE = docker-compose.yml

# Build all images
build:
	@echo "Building all images..."
	docker build -t $(SERVER_IMAGE) -f Dockerfile.server .
	docker build -t $(CLIENT_IMAGE) -f Dockerfile.client .

# Build only server
build-server:
	@echo "Building server image..."
	docker build -t $(SERVER_IMAGE) -f Dockerfile.server .

# Build only client
build-client:
	@echo "Building client image..."
	docker build -t $(CLIENT_IMAGE) -f Dockerfile.client .

# Run server
run-server:
	@echo "Starting server..."
	docker run -it --rm -p 5000:5000 -p 5001:5000 --name chat-server $(SERVER_IMAGE)

# Run client
run-client:
	@echo "Starting client..."
	docker run -it --rm --name chat-client $(CLIENT_IMAGE)

# Run with docker-compose
compose-up:
	@echo "Starting with docker-compose..."
	docker-compose -f $(COMPOSE_FILE) up -d

compose-down:
	@echo "Stopping docker-compose..."
	docker-compose -f $(COMPOSE_FILE) down

# View logs
logs:
	docker-compose -f $(COMPOSE_FILE) logs -f

# Clean up
clean:
	@echo "Cleaning up containers and images..."
	docker-compose -f $(COMPOSE_FILE) down --rmi all --volumes
	docker system prune -f

# Show help
help:
	@echo "Available commands:"
	@echo "  build          - Build all Docker images"
	@echo "  build-server   - Build only server image"
	@echo "  build-client   - Build only client image"
	@echo "  run-server     - Run server container"
	@echo "  run-client     - Run client container"
	@echo "  compose-up     - Start with docker-compose"
	@echo "  compose-down   - Stop docker-compose"
	@echo "  logs           - View logs"
	@echo "  clean          - Clean up everything"
	@echo "  help           - Show this help"
