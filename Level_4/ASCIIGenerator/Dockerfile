# Многоступенчатый Dockerfile для ASCII Name Generator
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Копируем файлы проекта
COPY *.csproj .
RUN dotnet restore

# Копируем весь исходный код
COPY . .

# Собираем приложение
RUN dotnet build -c Release -o /app/build --no-restore

# Публикуем приложение
RUN dotnet publish -c Release -o /app/publish --no-restore

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app

# Устанавливаем необходимые пакеты для работы с консолью
RUN apt-get update && apt-get install -y \
    locales \
    && rm -rf /var/lib/apt/lists/* \
    && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8

ENV LANG=en_US.utf8

# Копируем собранное приложение из стадии build
COPY --from=build /app/publish .

# Создаем volume для сохранения результатов
VOLUME /app/output

# Точка входа
ENTRYPOINT ["dotnet", "AsciiNameGenerator.dll"]
