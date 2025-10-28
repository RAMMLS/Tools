# ASCII Name Generator 🎨

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-6.0-purple?style=for-the-badge)
![Windows](https://img.shields.io/badge/Windows-10%2B-blue?style=for-the-badge)
![Console](https://img.shields.io/badge/Console-App-green?style=for-the-badge)

Профессиональный генератор ASCII-артов для создания стильных текстовых изображений. Превратите обычный текст в произведение искусства с помощью различных шрифтов и цветовых схем.

## ✨ Возможности

- 🎯 **Множество шрифтов** - Стандартные, жирные, блочные и декоративные
- 🌈 **Цветовые схемы** - Монохромные, радужные, градиентные
- 🖼️ **Стильное оформление** - Рамки, выравнивание, эффекты
- 💾 **Экспорт результатов** - Сохранение в TXT и HTML форматах
- 🎮 **Интерактивный интерфейс** - Простое управление через меню
- ⚡ **Быстрая генерация** - Мгновенное создание артов

## 🚀 Быстрый старт

### Требования
- .NET 6.0 Runtime или выше
- Windows/Linux/macOS
- Консоль с поддержкой UTF-8

### Установка
```bash
# Клонирование репозитория
git clone https://github.com/your-username/AsciiNameGenerator.git

# Компиляция
dotnet build --configuration Release

# Запуск
dotnet run


# Архитектура 

AsciiNameGenerator/
├── Program.cs                 # Главное приложение
├── AsciiArtGenerator.cs      # Ядро генерации
├── Fonts/                    # Библиотека шрифтов
│   ├── StandardFont.cs
│   ├── BoldFont.cs
│   └── BlockFont.cs
├── Styles/                   # Стили и анимации
│   ├── ColorStyle.cs
│   └── AnimationStyle.cs
├── Models/                   # Модели данных
│   ├── CharacterMap.cs
│   └── GenerationOptions.cs
└── Utilities/                # Вспомогательные классы
    ├── TextHelper.cs
    └── FileExport.cs


🚀 Запуск и тестирование
1. Базовый тест работы контейнера
bash

# Простой тест
docker run --rm ascii-name-generator "HELLO"

# Или с передачей аргументов
docker run --rm ascii-name-generator "TEST ASCII"

2. Интерактивный запуск (рекомендуется)
bash

# Запуск в интерактивном режиме
docker run -it --rm ascii-name-generator

3. Запуск с сохранением результатов
bash

# Создайте папку для результатов
mkdir -p ascii-output

# Запуск с volume для сохранения файлов
docker run -it --rm -v $(pwd)/ascii-output:/app/output ascii-name-generator

📋 Примеры команд для тестирования
Проверка различных функций:
bash

# 1. Проверка генерации ASCII артов
docker run --rm ascii-name-generator "HELLO"

# 2. Проверка работы с разными шрифтами (если реализовано)
docker run --rm ascii-name-generator "WORLD"

# 3. Тест сохранения файлов
docker run --rm -v $(pwd)/output:/app/output ascii-name-generator

🐳 Полезные Docker команды
Управление контейнерами:
bash

# Просмотр всех контейнеров
docker ps -a

# Просмотр образов
docker images

# Остановка всех контейнеров
docker stop $(docker ps -aq)

# Удаление всех остановленных контейнеров
docker rm $(docker ps -aq)

# Очистка системы Docker
docker system prune -f

Для разработки:
bash

# Запуск с разными переменными окружения
docker run -it -e DOTNET_ENVIRONMENT=Development ascii-name-generator

# Запуск с доступом к хостовой сети
docker run -it --network host ascii-name-generator

🔧 Если нужно внести изменения
Процесс обновления:
bash

# 1. Внесите изменения в код
# 2. Пересоберите образ
docker build -t ascii-name-generator .

# 3. Запустите обновленную версию
docker run -it --rm ascii-name-generator
