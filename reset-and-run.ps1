# Stop the application
Write-Host "Stopping application..." -ForegroundColor Yellow
Get-Process -Name "BookingSystem.Web" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

# Delete database files
Write-Host "Deleting database..." -ForegroundColor Yellow
Remove-Item "BookingSystem.Web\BookingSystem.db" -Force -ErrorAction SilentlyContinue
Remove-Item "BookingSystem.Web\BookingSystem.db-shm" -Force -ErrorAction SilentlyContinue
Remove-Item "BookingSystem.Web\BookingSystem.db-wal" -Force -ErrorAction SilentlyContinue

Write-Host "Database deleted" -ForegroundColor Green

# Run the application
Write-Host "Starting application..." -ForegroundColor Yellow
Set-Location BookingSystem.Web
dotnet run
