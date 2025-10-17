FROM gcc:latest

# Устанавливаем необходимые пакеты
RUN apt-get update && apt-get install -y \
    libpcap-dev \
    && rm -rf /var/lib/apt/lists/*

# Создаем рабочую директорию
WORKDIR /app

# Копируем оба исходных файла
COPY mail.c /app/
COPY sender.c /app/

# Компилируем программы
RUN gcc -o sender sender.c -lpcap
RUN gcc -o mail mail.c -lpcap

# Устанавливаем необходимые права
RUN chmod +x sender mail

# По умолчанию запускаем sender
CMD ["./sender"]
