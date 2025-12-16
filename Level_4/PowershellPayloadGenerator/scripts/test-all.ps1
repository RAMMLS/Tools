Write-Host "=== PowerShell Payload Generator Test Suite ===" -ForegroundColor Green
Write-Host "Testing all payload types..." -ForegroundColor Yellow

# Массив тестовых конфигураций
$tests = @(
    @{Type="Discovery"; Args="--type Discovery"},
    @{Type="SecurityAudit"; Args="--type SecurityAudit"},
    @{Type="Command"; Args="--type Command --command 'Get-Process'"},
    @{Type="PrivilegeEnumeration"; Args="--type PrivilegeEnumeration"},
    @{Type="VulnerabilityCheck"; Args="--type VulnerabilityCheck"},
    @{Type="PortScan"; Args="--type PortScan --ip 127.0.0.1"},
    @{Type="LogCollection"; Args="--type LogCollection"},
    @{Type="Download"; Args="--type Download --ip https://raw.githubusercontent.com/microsoft/dotnet/main/README.md"}
)

$results = @()

foreach ($test in $tests) {
    Write-Host "`nTesting: $($test.Type)" -ForegroundColor Cyan
    Write-Host "Command: docker run --rm payload-generator $($test.Args)" -ForegroundColor Gray
    
    $startTime = Get-Date
    try {
        $output = docker run --rm payload-generator $test.Args 2>&1
        $success = $LASTEXITCODE -eq 0
        $message = if ($success) { "SUCCESS" } else { "FAILED" }
        $color = if ($success) { "Green" } else { "Red" }
        
        Write-Host "Result: $message" -ForegroundColor $color
        
        # Сохраняем результат
        $result = [PSCustomObject]@{
            Test = $test.Type
            Success = $success
            Duration = (Get-Date) - $startTime
            OutputFile = "output/test_$($test.Type)_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"
        }
        
        $output | Out-File -FilePath $result.OutputFile -Encoding UTF8
        $results += $result
        
    } catch {
        Write-Host "ERROR: $_" -ForegroundColor Red
    }
}

# Вывод результатов
Write-Host "`n=== TEST RESULTS ===" -ForegroundColor Green
$results | Format-Table Test, Success, Duration, OutputFile -AutoSize

$passed = ($results | Where-Object { $_.Success }).Count
$total = $results.Count
Write-Host "Passed: $passed/$total" -ForegroundColor $(if ($passed -eq $total) { "Green" } else { "Yellow" })
