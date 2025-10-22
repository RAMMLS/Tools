FROM ubuntu:20.04

ENV DEBIAN_FRONTEND=noninteractive

# Установка Mono для Ubuntu 20.04 (focal)
RUN apt-get update && apt-get install -y --no-install-recommends \
    gnupg \
    ca-certificates \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
    && apt-get update && apt-get install -y --no-install-recommends \
    mono-complete \
    x11-apps \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY MouseTracker.cs /app/

# Компиляция приложения
RUN mcs -r:System.Windows.Forms -r:System.Drawing -r:System.Data MouseTracker.cs

ENV DISPLAY=:0
CMD ["mono", "MouseTracker.exe"]
