@echo off
cd /d "C:\Users\basev\Bohdana\OmaTehtava\BDD-project\BookingSystem.Api"
echo Запуск Booking System API...
echo.
dotnet run --urls "http://localhost:5000;https://localhost:5001"
pause
