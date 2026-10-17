@echo off
set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

echo Compiling Folder Colorizer...
"%CSC%" /target:winexe /out:"D:\Downloads\FolderColorizer\FolderColorizer.exe" /win32icon:"D:\Downloads\FolderColorizer\src\Assets\app_icon.ico" /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll "D:\Downloads\FolderColorizer\src\Program.cs" "D:\Downloads\FolderColorizer\src\Core\ShellHelper.cs" "D:\Downloads\FolderColorizer\src\Core\FolderManager.cs" "D:\Downloads\FolderColorizer\src\Core\LedgerManager.cs" "D:\Downloads\FolderColorizer\src\Core\RegistryManager.cs"

if %ERRORLEVEL% equ 0 (
    echo [SUCCESS] FolderColorizer.exe compiled successfully!
) else (
    echo [ERROR] Compilation failed with error code %ERRORLEVEL%.
)
