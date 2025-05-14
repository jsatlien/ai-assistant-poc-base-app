@echo off
echo Stopping Repair Manager Application...
echo.

echo Stopping Backend API...
taskkill /f /im dotnet.exe

echo.
echo Stopping Frontend...
taskkill /f /im node.exe

echo.
echo Both applications have been stopped.
echo.
echo Press any key to close this window
pause > nul
