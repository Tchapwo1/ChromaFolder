# 🔄 Pillar 3: Application & User Flow Specification — Folder Colorizer for Windows 11

## 1. Primary Workflow: Colorizing a Folder

```mermaid
flowchart TD
    A["User Right-Clicks Any Folder in File Explorer"] --> B{"Windows 11 Context Menu"}
    B -->|"Direct / Shift+F10 / Show More Options"| C["Hover over 'Folder Colorizer'"]
    C --> D["Submenu Displays 6 Named Colors + Reset"]
    D -->|"Select Color (e.g. Blue)"| E["Trigger FolderColorizer.exe apply <folderPath> blue"]
    
    subgraph Execution ["FolderColorizer Engine Execution (< 100ms)"]
        E --> F{"Validate Target Folder"}
        F -->|"Path Not Found or Unreachable"| ERR1["Show Error Dialog / Non-zero Exit"]
        F -->|"Access Denied (ACL/System)"| ERR2["Show User-Friendly Access Warning"]
        F -->|"Valid & Writable"| G["Backup existing desktop.ini if present"]
        G --> H["Write customized desktop.ini with IconResource"]
        H --> I["Set desktop.ini to Hidden + System (0x06)"]
        I --> J["Set target folder to Read-Only (0x01)"]
        J --> K["Record assignment in local JSON ledger"]
        K --> L["Call SHChangeNotify(SHCNE_UPDATEITEM, path)"]
    end
    
    L --> M["File Explorer Instantly Redraws Folder with Blue Icon"]
```

---

## 2. Reset Workflow: Restoring Default Appearance

```mermaid
flowchart TD
    A["User Right-Clicks Colorized Folder"] --> B["Select 'Folder Colorizer' -> 'Reset to Default'"]
    B --> C["Trigger FolderColorizer.exe reset <folderPath>"]
    
    subgraph ResetLogic ["Engine Reset Logic"]
        C --> D{"Look up Folder in Ledger"}
        D -->|"Found Prior State"| E["Restore Original desktop.ini Content"]
        D -->|"No Prior Customization"| F["Delete desktop.ini safely"]
        F --> G["Remove Read-Only attribute from Folder"]
        E --> H["Update Ledger (Mark as Reverted)"]
        G --> H
        H --> I["Call SHChangeNotify(SHCNE_UPDATEITEM, path)"]
    end
    
    I --> J["Folder Immediately Reverts to Standard Windows 11 Yellow Icon"]
```

---

## 3. Mandatory States & Error Matrix

| State | Trigger Condition | System Behavior & User Communication | Recovery Action |
|---|---|---|---|
| **Loading / In-Flight** | Color verb clicked in context menu | Execution completes within 50ms; silent, instant visual switch in File Explorer. | None needed (instant). |
| **Success** | `desktop.ini` applied & `SHChangeNotify` emitted | Folder icon updates on screen; ledger timestamp updated. | User can immediately re-color or reset. |
| **Access Denied** | Target is inside `C:\Windows`, `Program Files`, or has read-only ACL | Native Windows modal: *"Cannot colorize this folder because Windows has restricted its access. Please choose a folder inside your user profile or external drive."* | Leaves folder untouched. Zero corrupt state. |
| **Missing Path** | Folder was renamed or deleted between menu open and click | Native Windows modal: *"Folder not found. It may have been moved or renamed."* | Removes stale entry from ledger. |
| **Network Share** | Target is on SMB / UNC path (`\\server\share`) | Evaluates if share supports hidden attributes; applies if supported or alerts if unsupported by server. | Gracefully handles non-NTFS volumes. |

---

## 4. Uninstallation Flow
1. User runs `uninstall.bat` (or Windows Settings &rarr; Installed Apps).
2. Prompt: *"Do you want to reset all colored folders back to default Windows icons before uninstalling?"*
   - **Option 1 (Recommended)**: Iterates over `assignments.json`, removes `desktop.ini`, removes `+r` attribute, and refreshes Explorer.
   - **Option 2 (Retain)**: Leaves icons in `%LocalAppData%\FolderColorizer\icons` so colored folders remain visually intact.
3. Removes `HKEY_CURRENT_USER\Software\Classes\Directory\shell\FolderColorizer` registry keys.
4. Cleans up application binaries.
