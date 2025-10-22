#!/bin/bash

# Скрипт для сборки и запуска Mouse Tracker в Docker

echo "===Building docker image==="
docker build -t mouse-tracker .

echo "===Starting container==="
# Запуск контейнера с пробросом X11 для отображения GUI
docker run -it --rm \
  -e DISPLAY=$DISPLAY \
  -v /tmp/.X11-unix:/tmp/.X11-unix \
  --name mouse-tracker-app \
  mouse-tracker
