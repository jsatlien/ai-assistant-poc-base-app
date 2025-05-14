@echo off
echo Starting Repair Manager Application...
echo.

echo Starting Backend API...
start cmd /k "cd Backend\RepairManagerApi && dotnet run"

echo.
echo Starting Frontend...
start cmd /k "cd Frontend\repair-manager-frontend && npm run serve"

echo.
echo Both applications are starting. Please wait a moment...
echo Backend will be available at: https://localhost:7231
echo Frontend will be available at: http://localhost:8080
echo.
echo Press any key to close this window (applications will continue running in their own windows)
pause > nul
