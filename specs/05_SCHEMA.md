# 🗄️ Pillar 5: Data Schema & Registry Contract — Folder Colorizer for Windows 11

## 1. Assignment Ledger Schema (`assignments.json`)
Located at `%LocalAppData%\FolderColorizer\data\assignments.json`.

```json
{
  "$schema": "https://foldercolorizer.local/schema/assignments-v1.json",
  "version": "1.0.0",
  "lastUpdated": "2026-10-17T07:50:00Z",
  "assignments": [
    {
      "folderId": "fc_7b82f01a",
      "folderPath": "D:\\Projects\\ClientAlpha",
      "normalizedPath": "d:\\projects\\clientalpha",
      "color": "blue",
      "iconPath": "C:\\Users\\User\\AppData\\Local\\FolderColorizer\\icons\\folder_blue.ico",
      "originalState": {
        "hadDesktopIni": false,
        "originalDesktopIniContent": null,
        "originalFolderAttributes": 16
      },
      "createdAt": "2026-10-17T07:45:00Z",
      "lastVerifiedAt": "2026-10-17T07:45:00Z",
      "status": "active"
    }
  ]
}
```

### Field Definitions
- `folderId`: 8-character hex hash based on canonical path, used for stable indexing.
- `folderPath`: Exact user-facing path at time of assignment.
- `normalizedPath`: Case-insensitive, backslash-standardized path for fast lookup.
- `color`: Predefined identifier (`red`, `blue`, `green`, `yellow`, `orange`, `purple`).
- `iconPath`: Absolute path to assigned `.ico` asset.
- `originalState`: Complete snapshot of pre-existing `desktop.ini` and attributes to guarantee lossless restoration upon reset or uninstall.
- `status`: `"active"` (currently colored), `"reset"` (reverted to default), or `"missing"` (folder no longer exists on disk).

---

## 2. Windows 11 Registry Contract (HKCU)
All entries are placed in `HKEY_CURRENT_USER`, ensuring zero admin prompts during installation or operation.

### Root Menu Key
`HKCU\Software\Classes\Directory\shell\FolderColorizer`
- `(Default)` = `"Folder Colorizer"`
- `MUIVerb` = `"Folder Colorizer"`
- `SubCommands` = `""` (Signals a cascading submenu in the modern Windows shell)
- `Icon` = `"%LocalAppData%\FolderColorizer\icons\folder_blue.ico"`

### Submenu Items Key
`HKCU\Software\Classes\Directory\shell\FolderColorizer\shell\`

| Subkey Name | `MUIVerb` String | `Icon` Reference | Execution Command (`command\(Default)`) |
|---|---|---|---|
| `01_Red` | `Red` | `"%LocalAppData%\...\folder_red.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" red` |
| `02_Blue` | `Blue` | `"%LocalAppData%\...\folder_blue.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" blue` |
| `03_Green` | `Green` | `"%LocalAppData%\...\folder_green.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" green` |
| `04_Yellow` | `Yellow` | `"%LocalAppData%\...\folder_yellow.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" yellow` |
| `05_Orange` | `Orange` | `"%LocalAppData%\...\folder_orange.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" orange` |
| `06_Purple` | `Purple` | `"%LocalAppData%\...\folder_purple.ico"` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" apply "%1" purple` |
| `07_Reset` | `Reset to Default` | `shell32.dll,238` | `"%LocalAppData%\FolderColorizer\FolderColorizer.exe" reset "%1"` |
