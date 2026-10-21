# 🎨 ChromaFolder for Windows 11

> Lightweight, offline-first desktop utility that lets you assign distinct colors to folders directly from Windows 11 File Explorer. Zero bloat, zero telemetry, and 0 MB background RAM.

![Platform: Windows 11](https://img.shields.io/badge/Platform-Windows%2011%20x64-0078D4?logo=windows)
![License: MIT](https://img.shields.io/badge/License-MIT-green)
![Status: Build--Ready v1.0.0](https://img.shields.io/badge/Status-v1.0.0%20Beta-success)

---

## ⚡ Overview & Core Workflow

ChromaFolder integrates directly into the Windows 11 shell, allowing you to visually categorize and prioritize projects, documents, clients, and deadlines in seconds:

1. **Right-click** any folder in File Explorer.
2. Select **ChromaFolder** (available directly or via *Show more options* / `Shift+F10`).
3. Choose one of six curated colors:
   - 🔴 **Red** (`#DC2626`): Urgent, Deadlines, Critical Bugs
   - 🔵 **Blue** (`#2563EB`): Primary Projects, Codebases, Client Work
   - 🟢 **Green** (`#16A34A`): Completed, Approved, Finance, Invoices
   - 🟡 **Yellow** (`#EAB308`): In Review, Pending, Staging
   - 🟠 **Orange** (`#EA580C`): High Priority, Action Required
   - 🟣 **Purple** (`#9333EA`): Personal, Creative, Media
4. The folder immediately displays its custom icon in File Explorer.
5. Revert at any time: Right-click &rarr; **ChromaFolder** &rarr; **Reset to Default**.

---

## 🛠️ Technical Architecture

- **Customization Engine**: Uses the standard Windows Shell Class Info pattern (`desktop.ini` with dual `IconResource` and `IconFile` + `IconIndex` mappings to multi-resolution `.ico` assets).
- **Directory Attributes**: Applies `FILE_ATTRIBUTE_READONLY` (`attrib +r`) to the folder—the exact signal required by Windows NT Explorer to parse `desktop.ini`.
- **Instant Shell Redraw**: Dispatches native Win32 `SHChangeNotify(SHCNE_UPDATEITEM)` via P/Invoke to instantly refresh File Explorer without restarting `explorer.exe`. System-wide `SHCNE_ASSOCCHANGED` flushes are reserved strictly for install/uninstall events.
- **Zero Elevated Privileges**: Runs 100% in user space (`HKEY_CURRENT_USER\Software\Classes\Directory\shell\ChromaFolder`); never requires Administrator/UAC prompts.
- **Safety Ledger**: Maintains `%LocalAppData%\ChromaFolder\data\assignments.json` to safely restore pre-existing folder configurations upon reset or uninstallation.
- **0 MB Background Footprint**: Pure on-demand C# Win32 binary. No tray daemon, no background service, no telemetry.

---

## 🚀 Installation & Uninstallation

### Option A: Windows Package Manager (WinGet)
```powershell
winget install ChromaFolder
```

### Option B: One-Click Installer (Zero Admin Rights)
Download and run [`ChromaFolderSetup.exe`](https://github.com/Tchapwo1/ChromaFolder/releases/latest/download/ChromaFolderSetup.exe):
- Installs to `%LocalAppData%\ChromaFolder\`.
- Automatically registers the cascading context menu in `HKCU`.
- Registers in Windows Settings &rarr; *Installed Apps* for 1-click uninstallation.

---

## 💻 CLI & GUI Usage

```powershell
# Apply a color to a folder
ChromaFolder.exe apply "D:\Projects\UrgentTask" red

# Reset a folder to default Windows icon
ChromaFolder.exe reset "D:\Projects\UrgentTask"

# Register context menu in HKCU
ChromaFolder.exe register

# Unregister context menu from HKCU
ChromaFolder.exe unregister

# View current colorized folder ledger
ChromaFolder.exe status

# Launch the interactive Palette Preview & Management GUI
ChromaFolder.exe gui
```

---

## 🧪 Automated Test Suite

Run `tests\TestHarness.exe`:
- Basic color application & attribute verification
- All 6 colors verification
- Spaces & Unicode path support (`Folder with spaces & 漢字 日本語 🚀`)
- Non-destructive reset & user file preservation
- Color re-assignment / update
- Invalid color rejection
- Missing folder reporting
- Ledger audit trail persistence
- Registry context menu registration & cleanup
