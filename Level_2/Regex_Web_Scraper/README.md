# Regex Web Scraper

Приложение C# для извлечения данных из веб-сайтов с использованием регулярных выражений (Regex).

## Описание

Это консольное приложение, которое загружает HTML-контент веб-сайта, указанного в конфигурационном файле, и извлекает данные с использованием предоставленных Regex паттернов. Извлеченные данные затем сохраняются в текстовый файл.

## Структура проекта
RegexWebScraper/
├── Config/
│  ├── settings.json     // Конфигурация программы (URL сайта, паттерны Regex)
│  └── patterns.json     // Альтернативный вариант хранения Regex паттернов
├── Data/
│  ├── scraped_data.txt    // Файл для сохранения извлеченных данных
│  └── errors.log       // Лог ошибок
├── Modules/
│  ├── WebClientWrapper.cs  // Обертка для HttpClient (с обработкой ошибок)
│  ├── DataExtractor.cs    // Класс для извлечения данных с помощью Regex
│  ├── DataWriter.cs     // Класс для записи извлеченных данных в файл
├── Helpers/
│  └── StringHelper.cs    // Вспомогательные методы для работы со строками
├── Models/
│  └── ScrapedItem.cs     // Модель данных для извлеченных элементов
├── Program.cs        // Главный файл программы
├── RegexWebScraper.csproj  // Файл проекта Visual Studio
└── README.md         // Описание проекта

```

▌Зависимости

•  Microsoft.Extensions.Configuration
•  Microsoft.Extensions.Configuration.FileExtensions
•  Microsoft.Extensions.Configuration.Json
•  Microsoft.Extensions.DependencyInjection
•  Microsoft.Extensions.Logging
•  Microsoft.Extensions.Logging.Console
•  Microsoft.Extensions.Options

▌Как использовать

1. Склонируйте репозиторий.
2. Откройте проект в Visual Studio или другом IDE.
3. Установите необходимые NuGet пакеты.
4. Отредактируйте Config/settings.json, указав URL веб-сайта, Regex паттерны для извлечения данных и другие настройки.
5. Запустите приложение.
6. Извлеченные данные будут сохранены в Data/scraped_data.txt. Ошибки будут записаны в Data/errors.log.

▌Конфигурация

•  WebsiteUrl: URL веб-сайта для извлечения данных.
•  DataPattern: Regex паттерн для извлечения основных данных.
•  TitlePattern: Regex паттерн для извлечения заголовков (опционально).
•  EnableLogging: Включить/выключить логирование.
•  MaxRetries: Максимальное количество попыток при неудачном запросе.
•  RetryDelayMilliseconds: Задержка в миллисекундах между попытками.

▌Предостережения

•  Использование Regex для парсинга HTML может быть хрупким, так как структура веб-сайтов может изменяться.
•  Убедитесь, что ваши действия не нарушают правила веб-сайта.
•  Не злоупотребляйте web scraping, чтобы не перегружать серверы.

```
