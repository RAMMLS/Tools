Browser Info Service

Сервис для сбора и отображения информации о браузере пользователя через HTTP-запросы. Возвращает заголовки браузера, информацию о соединении и IP-адрес клиента.
🏗️ Структура проекта
text

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

🚀 Быстрый старт
Предварительные требования

    Docker

    .NET 8.0 SDK (для локальной разработки)

Запуск через Docker

    Сборка Docker образа:

bash

docker build -t browser-info-service .

    Запуск контейнера:

bash

docker run -d -p 8080:80 --name browser-info-container browser-info-service

    Проверка работы:

bash

curl http://localhost:8080/browserinfo

Локальная разработка

    Перейдите в директорию src:

bash

cd src

    Восстановите зависимости:

bash

dotnet restore

    Запустите приложение:

bash

dotnet run

    Приложение будет доступно по адресу: http://localhost:5000

📡 API Endpoints
GET /

    Описание: Основная страница сервиса

    Ответ: Browser Info Service is running. Use /browserinfo endpoint.

GET /browserinfo

    Описание: Возвращает подробную информацию о браузере и соединении

    Формат ответа: JSON

    Пример ответа:

json

{
  "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
  "acceptLanguage": "en-US,en;q=0.9",
  "acceptEncoding": "gzip, deflate, br",
  "connection": "keep-alive",
  "cacheControl": "max-age=0",
  "secFetchDest": "document",
  "remoteIp": "172.17.0.1",
  "timestamp": "2024-01-15T10:30:00Z"
}

🛠️ Управление контейнером
Проверка статуса
bash

docker ps

Просмотр логов
bash

docker logs browser-info-container

Просмотр логов в реальном времени
bash

docker logs -f browser-info-container

Остановка контейнера
bash

docker stop browser-info-container

Удаление контейнера
bash

docker rm browser-info-container

Перезапуск контейнера
bash

docker restart browser-info-container

🔧 Технические детали
Собираемая информация

    User-Agent: Информация о браузере и операционной системе

    Accept-Language: Предпочитаемые языки

    Accept-Encoding: Поддерживаемые методы сжатия

    Connection: Тип соединения

    Cache-Control: Настройки кэширования

    Sec-Fetch-Dest: Цель запроса (безопасность)

    Remote IP: IP-адрес клиента

    Timestamp: Время запроса

Используемые технологии

    ASP.NET Core 8.0

    Docker

    C# 12.0

🔒 Безопасность
Рекомендации по использованию

    Сервис не сохраняет собранные данные

    Используйте за reverse proxy (nginx, Apache) для корректного определения IP-адресов

    В продакшн-окружении рекомендуется настроить HTTPS

    Рассмотрите добавление аутентификации и rate limiting

Особенности реализации

    Обработка ошибок и логирование

    Расширяемая архитектура с использованием интерфейсов

    Контейнеризация для изоляции окружения

    Оптимизированные Docker слои для быстрой сборки

🐛 Диагностика проблем
Контейнер не запускается
bash

docker logs browser-info-container

Порт занят

Используйте другой порт:
bash

docker run -d -p 8081:80 --name browser-info-container browser-info-service

Ошибки сборки

Очистите кэш Docker:
bash

docker system prune -f
docker build --no-cache -t browser-info-service .

📄 Лицензия

Этот проект предназначен для образовательных целей. Используйте ответственно.
