# Dockerfile
FROM kalilinux/kali-rolling

# Устанавливаем только необходимое для запуска WinForms под Mono
RUN apt update && \
    DEBIAN_FRONTEND=noninteractive apt install -y \
        mono-complete \
        libx11-6 \
        libxext6 \
        libxrender1 \
        libxtst6 \
        libxi6 \
        x11-apps \
        fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*

# Переменная DISPLAY для X11 (пробрасывается из хоста)
ENV DISPLAY=:0

WORKDIR /app

# Копируем исходники
COPY src/ ./src/

# Собираем приложение
RUN cd src && \
    mcs -out:BashPayloadGenerator.exe \
        Program.cs MainForm.cs PayloadGenerator.cs \
        -r:System.Windows.Forms -r:System.Drawing

# Запуск
CMD ["mono", "src/BashPayloadGenerator.exe"]
