@echo off
setlocal EnableDelayedExpansion
title Folder Colorizer — Per-User Installer

echo ===================================================
echo     Folder Colorizer for Windows 11 — Installer    
echo ===================================================
echo.

set TARGET_DIR=%LOCALAPPDATA%\FolderColorizer
set SOURCE_DIR=%~dp0..

echo [1/4] Creating application directory in LocalAppData...
if not exist "%TARGET_DIR%" mkdir "%TARGET_DIR%"
if not exist "%TARGET_DIR%\icons" mkdir "%TARGET_DIR%\icons"
if not exist "%TARGET_DIR%\data" mkdir "%TARGET_DIR%\data"

echo [2/4] Deploying executable and high-resolution icons...
copy /Y "%SOURCE_DIR%\FolderColorizer.exe" "%TARGET_DIR%\FolderColorizer.exe" >nul
copy /Y "%SOURCE_DIR%\Assets\*.ico" "%TARGET_DIR%\icons\" >nul
copy /Y "%~dp0uninstall.bat" "%TARGET_DIR%\uninstall.bat" >nul

echo [3/4] Registering Windows File Explorer context menu (HKCU)...
"%TARGET_DIR%\FolderColorizer.exe" register

echo [4/4] Registering in Windows Installed Apps...
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "DisplayName" /t REG_SZ /d "Folder Colorizer for Windows 11" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "DisplayVersion" /t REG_SZ /d "1.0.0" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "Publisher" /t REG_SZ /d "Folder Colorizer Team" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "DisplayIcon" /t REG_SZ /d "%TARGET_DIR%\icons\app_icon.ico" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "UninstallString" /t REG_SZ /d "\"%TARGET_DIR%\uninstall.bat\"" /f >nul
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\FolderColorizer" /v "InstallLocation" /t REG_SZ /d "%TARGET_DIR%" /f >nul

echo.
echo ===================================================
echo   [SUCCESS] Folder Colorizer is ready to use!      
echo ===================================================
echo.
echo Workflow:
echo   1. Right-click any folder in File Explorer.
echo   2. Select 'Folder Colorizer' (or 'Show more options' -^> 'Folder Colorizer').
echo   3. Click Red, Blue, Green, Yellow, Orange, or Purple!
echo   4. To revert: Right-click -^> Folder Colorizer -^> Reset to Default.
echo.
pause
