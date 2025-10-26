# Browser Info Service

Сервис для сбора и отображения информации о браузере пользователя через HTTP-запросы.

## 📁 Структура проекта

```
BrowserInfoService/
├── src/
│   ├── BrowserInfoService.csproj
│   ├── Program.cs
│   ├── Models/
│   │   └── BrowserInfo.cs
│   ├── Services/
│   │   ├── IBrowserInfoService.cs
│   │   └── BrowserInfoCollector.cs
│   ├── Extensions/
│   │   └── HttpContextExtensions.cs
│   └── Controllers/
│       └── BrowserInfoController.cs
├── Dockerfile
└── README.md
```

## 🚀 Быстрый старт

### Запуск через Docker

1. **Сборка Docker образа:**
```bash
docker build -t browser-info-service .
```

2. **Запуск контейнера:**
```bash
docker run -d -p 8080:80 --name browser-info-container browser-info-service
```

3. **Проверка работы:**
```bash
curl http://localhost:8080/browserinfo
```

### Локальная разработка

```bash
cd src
dotnet run
```

## 📡 API Endpoints

- `GET /` - информация о сервисе
- `GET /browserinfo` - возвращает информацию о браузере в формате JSON

**Пример ответа:**
```json
{
  "userAgent": "Mozilla/5.0...",
  "acceptLanguage": "en-US",
  "acceptEncoding": "gzip, deflate",
  "connection": "keep-alive",
  "cacheControl": "max-age=0",
  "secFetchDest": "document",
  "remoteIp": "172.17.0.1",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## 🛠️ Управление контейнером

```bash
# Проверка статуса
docker ps

# Просмотр логов
docker logs browser-info-container

# Остановка контейнера
docker stop browser-info-container

# Удаление контейнера
docker rm browser-info-container
```

## 🔒 Безопасность

- Сервис не сохраняет собранные данные
- Рекомендуется использовать за reverse proxy
- В продакшн-окружении настройте HTTPS

## 🐛 Диагностика

При проблемах с портом:
```bash
docker run -d -p 8081:80 --name browser-info-container browser-info-service
```
