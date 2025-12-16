# PowerShell Payload Generator

ОБРАЗОВАТЕЛЬНЫЙ ПРОЕКТ - ТОЛЬКО ДЛЯ ЗАКОННОГО ИСПОЛЬЗОВАНИЯ

## Описание
Проект демонстрирует принципы создания защищенных PowerShell скриптов для:
- Автоматизации задач безопасности
- Пентестинга в контролируемых средах
- Образовательных целей

## Требования
- .NET 6.0+
- Docker (опционально)

## Использование
```bash
dotnet run -- --type ReverseShell --ip 192.168.1.100 --port 443 --encode


# Сначала соберите проект локально
cd src/PowerShellPayloadGenerator
dotnet restore
dotnet build

# Если успешно, попробуйте Docker
docker build -t payload-generator .
