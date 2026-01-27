# Booking System API Launcher
$apiPath = "C:\Users\basev\Bohdana\OmaTehtava\BDD-project\BookingSystem.Api"
$projectFile = "$apiPath\BookingSystem.Api.csproj"

Write-Host "======================================"
Write-Host "Booking System API Launcher"
Write-Host "======================================"
Write-Host ""

# Check if project exists
if (Test-Path $projectFile) {
    Write-Host "✓ Project файл знайдено: $projectFile" -ForegroundColor Green
    Write-Host ""
    Write-Host "Запуск API на http://localhost:5000"
    Write-Host "Swagger UI: http://localhost:5000/swagger"
    Write-Host ""
    Write-Host "Натисніть Ctrl+C щоб зупинити сервер"
    Write-Host "======================================"
    Write-Host ""
    
    # Run the API
    Set-Location $apiPath
    & dotnet run --no-build --urls "http://localhost:5000"
} else {
    Write-Host "✗ Помилка: Проект не знайдено!" -ForegroundColor Red
    Write-Host "Шукав за: $projectFile"
}
