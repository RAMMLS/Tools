FROM ubuntu:18.04

# Установка зависимостей
RUN apt-get update && apt-get install -y \
    tor \
    privoxy \
    python3 \
    python3-pip \
    wget \
    vim \
    && rm -rf /var/lib/apt/lists/*

# Настройка Tor
RUN echo "ControlPort 9051\n" \
         "HashedControlPassword 16:9529EB03A306DE6F60171DE514EA2FCD49235BAF1E1E55897209679683\n" \
         "CookieAuthentication 1" >> /etc/tor/torrc

# Настройка Privoxy
RUN echo "forward-socks5 / 127.0.0.1:9050 ." >> /etc/privoxy/config

# Копирование Python файлов
WORKDIR /app
COPY ConnectionManager.py /app/
COPY main_script.py /app/

# Установка Python зависимостей
RUN pip3 install stem beautifulsoup4

# Запуск сервисов и скрипта
CMD service tor start && \
    service privoxy start && \
    sleep 5 && \
    python3 /app/main_script.py
