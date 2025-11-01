.PHONY: build run run-privileged compose-up compose-down clean help

# Variables
IMAGE_NAME = simple-firewall
CONTAINER_NAME = simple-firewall
COMPOSE_FILE = docker-compose.yml

# Build image
build:
	@echo "Building Simple Firewall image..."
	docker build -t $(IMAGE_NAME) .

# Run container (basic)
run:
	@echo "Running Simple Firewall..."
	docker run -it --rm \
		--name $(CONTAINER_NAME) \
		-p 8080:8080 \
		-v $(PWD)/logs:/app/logs \
		$(IMAGE_NAME)

# Run with privileged mode (for packet capture)
run-privileged:
	@echo "Running Simple Firewall with privileged mode..."
	docker run -it --rm \
		--name $(CONTAINER_NAME) \
		-p 8080:8080 \
		--cap-add=NET_ADMIN \
		--cap-add=NET_RAW \
		--privileged \
		-v $(PWD)/logs:/app/logs \
		$(IMAGE_NAME)

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
	@echo "Cleaning up..."
	docker-compose -f $(COMPOSE_FILE) down --rmi all --volumes
	docker system prune -f

# Test connectivity
test:
	@echo "Testing firewall web interface..."
	curl http://localhost:8080 || echo "Web interface not available"

# Show help
help:
	@echo "Available commands:"
	@echo "  build           - Build Docker image"
	@echo "  run             - Run container (basic)"
	@echo "  run-privileged  - Run with privileged mode"
	@echo "  compose-up      - Start with docker-compose"
	@echo "  compose-down    - Stop docker-compose"
	@echo "  logs            - View logs"
	@echo "  clean           - Clean up everything"
	@echo "  test            - Test web interface"
	@echo "  help            - Show this help"
