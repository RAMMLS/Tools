=== Генератор PowerShell Полезных Нагрузок ===
ТОЛЬКО ДЛЯ ОБРАЗОВАТЕЛЬНЫХ И ЗАКОННЫХ ЦЕЛЕЙ!

Конфигурация: Type: SecurityAudit
IP Address: Not set
Port: 443
Command: Get-Process
Encode: False
Obfuscate: False
Validate: True
Timeout: 30s
Use HTTPS: False


=== РЕЗУЛЬТАТ ГЕНЕРАЦИИ ===
Статус: УСПЕШНО
Уровень риска: Info
Размер: 991 байт
Время генерации: 8 мс
Хэш SHA256: 39cc36bfa98d686e37c489602d23a6d2b467db5542f5d73727d8741080a8afc0

=== СГЕНЕРИРОВАННАЯ НАГРУЗКА ===
================================================================================

# Security Audit Script
Write-Host '=== SECURITY AUDIT REPORT ===' -ForegroundColor Red -BackgroundColor White
Write-Host 'Generated: ' (Get-Date) -ForegroundColor Yellow
Write-Host '================================' -ForegroundColor Red

# 1. Account Policies
Write-Host '`n1. ACCOUNT POLICIES:' -ForegroundColor Cyan
net accounts

# 2. Password Policy
Write-Host '`n2. PASSWORD POLICY:' -ForegroundColor Cyan
net accounts | Select-String 'password'

# 3. Audit Policy
Write-Host '`n3. AUDIT POLICY:' -ForegroundColor Cyan
auditpol /get /category:*

# 4. Services Configuration
Write-Host '`n4. DANGEROUS SERVICES:' -ForegroundColor Cyan
Get-Service | Where-Object { 
    $_.Name -like '*telnet*' -or 
    $_.Name -like '*ftp*' -or 
    $_.Name -like '*smb*' 
} | Format-Table Name, Status, StartType

# 5. Open Shares
Write-Host '`n5. NETWORK SHARES:' -ForegroundColor Cyan
Get-SmbShare | Format-Table Name, Path, Description

Write-Host '`n=== AUDIT COMPLETE ===' -ForegroundColor Green

================================================================================

Сохранить нагрузку в файл? (y/n): 