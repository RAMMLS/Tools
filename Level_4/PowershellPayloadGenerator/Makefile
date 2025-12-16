.PHONY: build run test clean deploy help

# Variables
IMAGE_NAME = payload-generator
TAG = latest
CONTAINER_NAME = payload-gen

help:
	@echo "Available commands:"
	@echo "  make build        - Build Docker image"
	@echo "  make run          - Run container interactively"
	@echo "  make test         - Run all tests"
	@echo "  make clean        - Clean up containers and images"
	@echo "  make deploy       - Deploy with docker-compose"
	@echo "  make monitor      - Start monitoring"
	@echo "  make shell        - Open shell in container"

build:
	docker build -t $(IMAGE_NAME):$(TAG) .

run:
	docker run -it --rm \
		--name $(CONTAINER_NAME) \
		-v $(PWD)/output:/app/output \
		$(IMAGE_NAME):$(TAG)

test:
	docker run --rm $(IMAGE_NAME):$(TAG) --type Discovery --validate
	docker run --rm $(IMAGE_NAME):$(TAG) --type SecurityAudit
	docker run --rm $(IMAGE_NAME):$(TAG) --type Command --command "Get-Service"

clean:
	docker ps -aq --filter "name=$(CONTAINER_NAME)" | xargs docker rm -f
	docker images -q $(IMAGE_NAME) | xargs docker rmi -f
	docker system prune -f

deploy:
	docker-compose up -d
	@echo "Deployed! Check status with: docker-compose ps"

monitor:
	docker-compose logs -f generator

shell:
	docker exec -it $(CONTAINER_NAME) pwsh

benchmark:
	@echo "Running benchmark tests..."
	@for i in 1 2 3; do \
		echo "Test $$i:"; \
		time docker run --rm $(IMAGE_NAME):$(TAG) --type Discovery > /dev/null; \
	done

scan:
	@echo "Security scan..."
	docker scan $(IMAGE_NAME):$(TAG)
