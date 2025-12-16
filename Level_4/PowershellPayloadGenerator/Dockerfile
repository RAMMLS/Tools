FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Создаем рабочую директорию
WORKDIR /src

# Копируем все файлы проекта
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj"

# Собираем проект
RUN dotnet build "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj" -c Release -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PowerShellPayloadGenerator.dll"]
