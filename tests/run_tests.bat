@echo off
setlocal
cd /d "%~dp0"

echo Compiling Test Harness...
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:exe /out:"TestHarness.exe" /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll "TestHarness.cs" "..\src\Core\ShellHelper.cs" "..\src\Core\FolderManager.cs" "..\src\Core\LedgerManager.cs" "..\src\Core\RegistryManager.cs" >nul

if %ERRORLEVEL% neq 0 (
    echo [ERROR] Test compilation failed!
    exit /b 1
)

echo Running Test Suite...
TestHarness.exe
