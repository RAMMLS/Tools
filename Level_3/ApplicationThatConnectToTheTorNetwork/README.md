Сборка и запуск:
# Собрать образ
docker build -t tor-parser .

# Запустить контейнер
docker run -it --rm tor-parser


Альтернативный способ запуска с мониторингом:
# Запуск с интерактивным терминалом
docker run -it tor-parser /bin/bash

# Внутри контейнера можно запускать сервисы вручную:
service tor start
service privoxy start
python3 /app/main_script.py

Для отладки:
# Проверить логи Tor
docker exec -it <container_id> tail -f /var/log/tor/log

# Проверить работу Tor
docker exec -it <container_id> curl --socks5 localhost:9050 http://icanhazip.com/
