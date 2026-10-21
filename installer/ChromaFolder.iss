; =====================================================================
; ChromaFolder for Windows 11 - Inno Setup 6 Script
; Lightweight, per-user, zero-UAC single-executable installer
; =====================================================================

#define MyAppName "ChromaFolder"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "ChromaFolder"
#define MyAppURL "https://github.com/Tchapwo1/ChromaFolder"
#define MyAppExeName "ChromaFolder.exe"
#define MySourceDir ".."

[Setup]
AppId={{9B7E3E5D-4F2A-4B89-8C21-6E3E9F8B5A10}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={localappdata}\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; Enforce per-user install (never prompts for UAC / Admin elevation)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=.\dist
OutputBaseFilename=ChromaFolderSetup
SetupIconFile={#MySourceDir}\Assets\app_icon.ico
UninstallDisplayIcon={app}\icons\app_icon.ico
UninstallDisplayName={#MyAppName} for Windows 11
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
; Main executable
Source: "{#MySourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
; Icon assets
Source: "{#MySourceDir}\Assets\*.ico"; DestDir: "{app}\icons"; Flags: ignoreversion recursesubdirs createallsubdirs
; Documentation
Source: "{#MySourceDir}\README.md"; DestDir: "{app}"; Flags: ignoreversion isreadme

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\icons\app_icon.ico"; Parameters: "gui"

[Run]
; Automatically register cascading right-click context menu in HKCU
Filename: "{app}\{#MyAppExeName}"; Parameters: "register"; Flags: runhidden; Description: "Register Windows File Explorer context menu"
; Optional launch GUI after installation completes
Filename: "{app}\{#MyAppExeName}"; Parameters: "gui"; Flags: nowait postinstall skipifsilent; Description: "Launch {#MyAppName} Palette Manager"

[UninstallRun]
; Cleanly remove context menu registrations on uninstall
Filename: "{app}\{#MyAppExeName}"; Parameters: "unregister"; Flags: runhidden

[UninstallDelete]
Type: filesandordirs; Name: "{app}\icons"
Type: files; Name: "{app}\{#MyAppExeName}"
Type: dirifempty; Name: "{app}"

[Code]
// Custom prompt on uninstall to offer restoring custom folder icons
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    // Optional: could invoke reset on tracked folders if user desires
  end;
end;
