FROM python:3.9-slim

RUN apt-get update && apt-get install -y \
    gcc \
    g++ \
    python3-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY main.py /app/
COPY scrubbed.csv /app/

# Создаем папку для графиков в контейнере
RUN mkdir -p /app/ufo_graphs

ENV MPLBACKEND=Agg
ENV YANDEX_API_KEY="test_key"

CMD ["python", "main.py"]
