FROM ubuntu:22.04

# Установка переменной окружения для избежания интерактивных запросов
ENV DEBIAN_FRONTEND=noninteractive

# Обновление и установка только необходимых пакетов
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    wget \
    gnupg \
    && wget -qO- https://download.mono-project.com/repo/xamarin.gpg | apt-key add - \
    && echo "deb https://download.mono-project.com/repo/ubuntu stable-jammy main" > /etc/apt/sources.list.d/mono-official-stable.list \
    && apt-get update && apt-get install -y --no-install-recommends \
    mono-runtime \
    mono-mcs \
    libmono-winforms2.0-cil \
    libmono-system-windows-forms4.0-cil \
    x11-apps \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY MouseTracker.cs /app/

# Компиляция приложения
RUN mcs -r:System.Windows.Forms -r:System.Drawing -r:System.Data MouseTracker.cs

ENV DISPLAY=:0
CMD ["mono", "MouseTracker.exe"]
