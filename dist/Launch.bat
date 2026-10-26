@echo off
rem ---------------------------------------------------------------------------
rem  SN Skin Unlocker - convenience launcher
rem  Elevates, clears the previous extraction, starts SNLauncher.exe
rem ---------------------------------------------------------------------------
setlocal
cd /d "%~dp0"

net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting administrator privileges ...
    powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
    exit /b
)

if not exist "SNLauncher.exe" (
    echo [ERROR] SNLauncher.exe not found in this folder.
    pause
    exit /b 1
)
if not exist "snarl_inner.dll" (
    echo [ERROR] snarl_inner.dll not found in this folder.
    pause
    exit /b 1
)

echo Cleaning previous extraction ...
rd /s /q "%LOCALAPPDATA%\Temp\SnarlV3" 2>nul

echo Starting ...
start "" "%~dp0SNLauncher.exe"

echo.
echo Launcher started. Pick a mode in the Snarl window.
echo Keep this window / the launcher process running - it applies the patches.
echo.
timeout /t 6 >nul
