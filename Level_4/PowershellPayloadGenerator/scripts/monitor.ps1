# Мониторинг работы генератора
Write-Host "=== Payload Generator Monitor ===" -ForegroundColor Green
Write-Host "Monitoring container activity..." -ForegroundColor Yellow

function Show-ContainerStatus {
    $containers = docker ps -a --filter "name=payload" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
    Write-Host "`nCurrent Containers:" -ForegroundColor Cyan
    $containers
}

function Show-ImageInfo {
    $images = docker images payload-generator* --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}"
    Write-Host "`nAvailable Images:" -ForegroundColor Cyan
    $images
}

function Run-HealthCheck {
    Write-Host "`nRunning health check..." -ForegroundColor Magenta
    
    $healthTests = @(
        @{Name="Basic Discovery"; Command="docker run --rm payload-generator --type Discovery --validate"}
        @{Name="Security Audit"; Command="docker run --rm payload-generator --type SecurityAudit"}
        @{Name="Build Check"; Command="docker build -t payload-generator-test ."}
    )
    
    foreach ($test in $healthTests) {
        Write-Host "  Testing: $($test.Name)" -NoNewline
        try {
            Invoke-Expression $test.Command > $null 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host " [✓]" -ForegroundColor Green
            } else {
                Write-Host " [✗]" -ForegroundColor Red
            }
        } catch {
            Write-Host " [✗]" -ForegroundColor Red
        }
    }
}

function Monitor-Resources {
    Write-Host "`nSystem Resources:" -ForegroundColor Cyan
    docker stats --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}" | Select-Object -First 5
}

# Основной цикл мониторинга
while ($true) {
    Clear-Host
    Write-Host "=== Payload Generator Monitor ===" -ForegroundColor Green
    Write-Host "Press Ctrl+C to exit`n" -ForegroundColor Yellow
    
    Show-ContainerStatus
    Show-ImageInfo
    Monitor-Resources
    
    # Запускаем проверку каждые 30 секунд
    Start-Sleep -Seconds 30
}
