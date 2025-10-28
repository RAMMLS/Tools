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
