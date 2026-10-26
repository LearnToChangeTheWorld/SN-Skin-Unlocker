@echo off
rem ---------------------------------------------------------------------------
rem  SN Skin Unlocker - build script
rem  Compiles SNLauncher.cs with the in-box C# compiler (no SDK required)
rem  and embeds app.manifest (requireAdministrator).
rem ---------------------------------------------------------------------------
setlocal
cd /d "%~dp0"

set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo [ERROR] csc.exe not found. .NET Framework 4.x is required.
    pause
    exit /b 1
)

if not exist "..\dist" mkdir "..\dist"

"%CSC%" /nologo /target:exe /codepage:65001 /optimize+ ^
        /win32manifest:app.manifest ^
        /out:"..\dist\SNLauncher.exe" ^
        SNLauncher.cs

if errorlevel 1 (
    echo.
    echo [ERROR] build failed.
    pause
    exit /b 1
)

echo.
echo [OK] built: ..\dist\SNLauncher.exe
echo.
pause
