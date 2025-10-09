#!/bin/bash 

BASE_DIR = $(realpath "$(dirname "$BASH_SOURCE")")
if [[ ! -d "BASH_DIR/auth"]]; then 
  echo "Creating auth dir..."
  mkdir -p "$BASH_DIR/auth"
fi 

CONTAINER = "main"
IMAGE = "RAMMLS/Tools:latest"
IMG_MIRROR = "ghcr.io/RAMMLS/Tools:latest"
MOUNT_LOCATION = ${BASE_DIR}/auth
check_container = $(docker ps --all --format "{{.Names}}")

if [[ ! $check_container == $CONTAINER]]; then 
  echo "Creating new container"
  docker create \
    --interactive --tty \
    --volume ${MOUNT_LOCATION}:/Tools/Level_3/HTTPSServerForPhishing/auth \
    --newtwork host \
    --name "${CONTAINER}" \
    "${IMAGE}"
fi 

docker start --interactive "${CONTAINER}"
