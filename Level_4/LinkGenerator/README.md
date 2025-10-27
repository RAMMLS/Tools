# Link Generator

Простое приложение на C#, которое генерирует текстовые файлы со ссылками.

## Сборка и запуск

### Локально
1. Установите [.NET 6 SDK](https://dotnet.microsoft.com/download)
2. Выполните в терминале:
```bash
dotnet run


[1] Соберите образ 
docker build -t link-generator .

[2] Запустите контейнер 
docker run --rm link-generator
