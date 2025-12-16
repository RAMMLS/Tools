# Простой скрипт для сборки проекта
Write-Host "=== PowerShell Payload Generator Build ===" -ForegroundColor Green

# Проверяем структуру проекта
Write-Host "Checking project structure..." -ForegroundColor Yellow

$requiredDirs = @(
    "src/PowerShellPayloadGenerator/Models",
    "src/PowerShellPayloadGenerator/Services", 
    "src/PowerShellPayloadGenerator/Utilities"
)

foreach ($dir in $requiredDirs) {
    if (-not (Test-Path $dir)) {
        Write-Host "Creating directory: $dir" -ForegroundColor Cyan
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}

# Создаем недостающие файлы
$projectFile = "src/PowerShellPayloadGenerator/PowerShellPayloadGenerator.csproj"
if (-not (Test-Path $projectFile)) {
    Write-Host "Creating project file..." -ForegroundColor Cyan
    $projectContent = @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
'@
    Set-Content -Path $projectFile -Value $projectContent
}

# Проверяем наличие SecurityHelper.cs
$securityHelperFile = "src/PowerShellPayloadGenerator/Utilities/SecurityHelper.cs"
if (-not (Test-Path $securityHelperFile)) {
    Write-Host "Creating SecurityHelper.cs..." -ForegroundColor Cyan
    # Копируем содержимое из вышеуказанного кода
    # или создаем упрощенную версию
    $helperContent = @'
using System;
using System.Text;
using System.Security.Cryptography;

namespace PowerShellPayloadGenerator.Utilities
{
    public static class SecurityHelper
    {
        public static string EncodeBase64(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
        
        public static string CalculateSha256Hash(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        
        public static string ObfuscatePowerShell(string script)
        {
            return script; // Упрощенная версия
        }
    }
}
'@
    Set-Content -Path $securityHelperFile -Value $helperContent
}

# Собираем проект
Write-Host "Building project..." -ForegroundColor Green
Set-Location "src/PowerShellPayloadGenerator"
dotnet restore
dotnet build -c Release

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host "Run: dotnet run -- --type Discovery" -ForegroundColor Cyan
} else {
    Write-Host "Build failed!" -ForegroundColor Red
}
