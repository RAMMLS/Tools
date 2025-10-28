# Многоступенчатый Dockerfile для ASCII Name Generator
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["AsciiNameGenerator.csproj", "."]
RUN dotnet restore "AsciiNameGenerator.csproj"

# Копируем весь исходный код
COPY . .
WORKDIR "/src"

# Собираем приложение
RUN dotnet build "AsciiNameGenerator.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "AsciiNameGenerator.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app

# Устанавливаем необходимые пакеты для работы с консолью
RUN apt-get update && apt-get install -y \
    locales \
    && rm -rf /var/lib/apt/lists/* \
    && localedef -i en_US -c -f UTF-8 -A /usr/share/locale/locale.alias en_US.UTF-8

ENV LANG=en_US.utf8

# Копируем собранное приложение
COPY --from=publish /app/publish .

# Создаем volume для сохранения результатов
VOLUME /app/output

# Точка входа
ENTRYPOINT ["dotnet", "AsciiNameGenerator.dll"]
