FROM ubuntu:22.04

# Устанавливаем переменные окружения для избежания интерактивных диалогов
ENV DEBIAN_FRONTEND=noninteractive

# Обновляем и устанавливаем базовые пакеты
RUN apt-get update && apt-get install -y \
    wget \
    git \
    build-essential \
    software-properties-common \
    && rm -rf /var/lib/apt/lists/*

# Устанавливаем mingw-w64 для C++ кросс-компиляции
RUN apt-get update && apt-get install -y \
    mingw-w64 \
    && rm -rf /var/lib/apt/lists/*

# Устанавливаем Go
RUN wget https://golang.org/dl/go1.21.0.linux-amd64.tar.gz && \
    tar -C /usr/local -xzf go1.21.0.linux-amd64.tar.gz && \
    rm go1.21.0.linux-amd64.tar.gz
ENV PATH="/usr/local/go/bin:${PATH}"
ENV GOPATH=/go
ENV PATH="${GOPATH}/bin:${PATH}"

# Создаем рабочую директорию
WORKDIR /workspace

# Создаем тестовые файлы для проверки
COPY <<'EOF' /workspace/test.cpp
#include <iostream>
using namespace std;

int main() {
    cout << "Hello from Windows (C++)!" << endl;
    return 0;
}
EOF

COPY <<'EOF' /workspace/test.go
package main

import "fmt"

func main() {
    fmt.Println("Hello from Windows (Go)!")
}
EOF

# Создаем скрипт для автоматической сборки
COPY <<'EOF' /usr/local/bin/build-all.sh
#!/bin/bash
echo "=== Testing C++ Compilation ==="
x86_64-w64-mingw32-g++ --version

echo "=== Testing Go Compilation ==="
go version

echo "=== Building Test Programs ==="

# Компилируем C++ программу
echo "Building C++..."
x86_64-w64-mingw32-g++ -static -o /workspace/hello-cpp.exe /workspace/test.cpp

# Компилируем Go программу
echo "Building Go..."
cd /workspace && GOOS=windows GOARCH=amd64 go mod init testapp && go build -o /workspace/hello-go.exe /workspace/test.go

echo "=== Verification ==="
file /workspace/*.exe

echo "=== Build Complete ==="
ls -la /workspace/*.exe
EOF

RUN chmod +x /usr/local/bin/build-all.sh

# Устанавливаем команду по умолчанию
CMD ["/bin/bash"]
