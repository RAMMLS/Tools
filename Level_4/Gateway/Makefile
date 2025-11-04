.PHONY: build build-gateway build-services compose-up compose-down clean test help

# Variables
GATEWAY_IMAGE = api-gateway
SERVICE_A_IMAGE = service-a
SERVICE_B_IMAGE = service-b
COMPOSE_FILE = docker-compose.yml

# Build all images
build:
	@echo "Building all images..."
	docker-compose -f $(COMPOSE_FILE) build

# Build only gateway
build-gateway:
	@echo "Building gateway image..."
	docker build -t $(GATEWAY_IMAGE) -f Dockerfile.gateway .

# Build only services
build-services:
	@echo "Building service images..."
	docker build -t $(SERVICE_A_IMAGE) -f Dockerfile.service-a .
	docker build -t $(SERVICE_B_IMAGE) -f Dockerfile.service-b .

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

logs-gateway:
	docker-compose -f $(COMPOSE_FILE) logs -f gateway

logs-service-a:
	docker-compose -f $(COMPOSE_FILE) logs -f service-a

logs-service-b:
	docker-compose -f $(COMPOSE_FILE) logs -f service-b

# Clean up
clean:
	@echo "Cleaning up..."
	docker-compose -f $(COMPOSE_FILE) down --rmi all --volumes
	docker system prune -f

# Test connectivity
test:
	@echo "Testing gateway..."
	curl http://localhost:5000/health || echo "Gateway not available"
	@echo "Testing service-a..."
	docker exec service-a curl -s http://localhost:5001/health || echo "Service A not available"
	@echo "Testing service-b..."
	docker exec service-b curl -s http://localhost:5002/health || echo "Service B not available"

# Show help
help:
	@echo "Available commands:"
	@echo "  build           - Build all Docker images"
	@echo "  build-gateway   - Build only gateway"
	@echo "  build-services  - Build only services"
	@echo "  compose-up      - Start with docker-compose"
	@echo "  compose-down    - Stop docker-compose"
	@echo "  logs            - View all logs"
	@echo "  logs-gateway    - View gateway logs"
	@echo "  logs-service-a  - View service A logs"
	@echo "  logs-service-b  - View service B logs"
	@echo "  clean           - Clean up everything"
	@echo "  test            - Test all services"
	@echo "  help            - Show this help"
