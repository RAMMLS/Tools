Passive Web Scanner 🔍

Простой, но мощный пассивный веб-сканер безопасности, написанный на C#. Инструмент анализирует HTTP-трафик на наличие уязвимостей и конфиденциальных данных без активного воздействия на целевые системы.

https://img.shields.io/badge/C%2523-239120?style=for-the-badge&logo=c-sharp&logoColor=white
https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white
https://img.shields.io/badge/Security-Expert-green?style=for-the-badge
📋 Оглавление

    Возможности

    Быстрый старт

    Установка

    Использование

    Примеры

    Структура проекта

    Типы обнаруживаемых угроз

    Разработка

    Безопасность

    Лицензия

🚀 Возможности

    🔒 Пассивное сканирование - Анализ без воздействия на целевые системы

    📁 Поддержка файловой системы - Работа с HTTP-запросами и ответами

    🎯 Обнаружение уязвимостей - SQL-инъекции, XSS, Path Traversal

    📊 Анализ чувствительных данных - Email, SSN, кредитные карты

    🔍 Проверка заголовков - Безопасность cookies и заголовков сервера

    📝 Генерация отчетов - Детализированный вывод результатов

⚡ Быстрый старт
bash

# Клонирование репозитория
git clone https://github.com/your-username/passive-web-scanner.git
cd passive-web-scanner

# Компиляция проекта
dotnet build

# Запуск сканера
dotnet run

📥 Установка
Требования

    .NET 6.0 или выше

    Windows/Linux/macOS

Установка из исходного кода

    Скачайте исходный код

    Измените конфигурацию при необходимости

    Соберите проект:

bash

dotnet build --configuration Release

🎮 Использование
Базовое использование
csharp

// Создание экземпляра сканера
var scanner = new PassiveWebScanner();

// Анализ отдельного файла
scanner.AnalyzeFile("request.txt");

// Анализ директории
scanner.AnalyzeDirectory("./http-traffic/");

Командная строка
bash

# Анализ конкретного файла
PassiveWebScanner --file request1.txt

# Анализ всей директории
PassiveWebScanner --dir ./traffic-logs/

# Подробный вывод
PassiveWebScanner --file response.txt --verbose

📊 Примеры
Пример вывода
text

Анализ файла: request1.txt
[!] Обнаружены возможные SQL-инъекции: 1
[!] Найдены чувствительные данные: Email (2)

Анализ файла: response2.txt
[!] Обнаружены SSN: 1
[!] Раскрытие информации о сервере
[!] Cookie без флага HttpOnly

Тестовые файлы

Сканер включает 10 тестовых файлов для демонстрации:
Файл	Тип	Обнаруживаемые угрозы
request1.txt	SQL-инъекция	SQLi, логин форма
response1.txt	Ответ сервера	Инфо о сервере, cookies
request2.txt	XSS атака	XSS, скрипты
response2.txt	JSON ответ	Email, SSN данные
request3.txt	Path Traversal	Directory traversal
