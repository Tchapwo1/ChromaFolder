@echo off
setlocal EnableDelayedExpansion
title Folder Colorizer — Uninstaller

echo ===================================================
echo     Folder Colorizer for Windows 11 — Uninstaller  
echo ===================================================
echo.

set TARGET_DIR=%LOCALAPPDATA%\FolderColorizer

if not exist "%TARGET_DIR%\FolderColorizer.exe" (
    echo [ERROR] Folder Colorizer is not installed in %TARGET_DIR%.
    pause
    exit /b 1
)

echo Would you like to restore all colored folders back to default
echo Windows folder icons before uninstalling? (Recommended)
echo.
set /p RESTORE_CHOICE="Restore folders? [Y/N, default Y]: "
if "%RESTORE_CHOICE%"=="" set RESTORE_CHOICE=Y
if /i "%RESTORE_CHOICE%"=="Y" (
    echo.
    echo Restoring colored folders from ledger...
    for /f "tokens=*" %%A in ('powershell -NoProfile -Command "if (Test-Path '%TARGET_DIR%\data\assignments.json') { (Get-Content '%TARGET_DIR%\data\assignments.json' | ConvertFrom-Json).assignments | Where-Object { $_.status -eq 'active' } | Select-Object -ExpandProperty folderPath }"') do (
        echo   - Reverting: %%A
        "%TARGET_DIR%\FolderColorizer.exe" reset "%%A" >nul 2>&1
    )
    echo Folders reverted to default.
)

echo.
echo [1/3] Removing File Explorer context menu...
"%TARGET_DIR%\FolderColorizer.exe" unregister >nul 2>&1

echo [2/3] Removing from Windows Installed Apps...
reg delete "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /f >nul 2>&1

echo [3/3] Removing application files and icons...
cd /d "%TEMP%"
rmdir /s /q "%TARGET_DIR%" >nul 2>&1

echo.
echo ===================================================
echo   [SUCCESS] Folder Colorizer cleanly uninstalled!  
echo ===================================================
echo.
pause
