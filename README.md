# Folder Colorizer for Windows 11

> Lightweight, offline-first desktop utility that lets you assign distinct colors to folders directly from Windows 11 File Explorer.

![Platform: Windows 11](https://img.shields.io/badge/Platform-Windows%2011%20x64-0078D4?logo=windows)
![License: MIT](https://img.shields.io/badge/License-MIT-green)
![Status: Build--Ready MVP](https://img.shields.io/badge/Status-Build--Ready%20MVP-success)

---

## 🎨 Overview & Core Workflow

Folder Colorizer integrates directly into the Windows 11 shell, allowing you to visually categorize and prioritize projects, documents, clients, and deadlines in seconds:

1. **Right-click** any folder in File Explorer.
2. Select **Folder Colorizer** (available directly or via *Show more options* / `Shift+F10`).
3. Choose one of six curated colors:
   - 🔴 **Red** (`#DC2626`): Urgent, Deadlines, Critical Bugs
   - 🔵 **Blue** (`#2563EB`): Primary Projects, Codebases, Client Work
   - 🟢 **Green** (`#16A34A`): Completed, Approved, Finance, Invoices
   - 🟡 **Yellow** (`#EAB308`): In Review, Pending, Staging
   - 🟠 **Orange** (`#EA580C`): High Priority, Action Required
   - 🟣 **Purple** (`#9333EA`): Personal, Creative, Media
4. The folder immediately displays its custom icon in File Explorer.
5. Revert at any time: Right-click &rarr; **Folder Colorizer** &rarr; **Reset to Default**.

---

## 🛠️ Technical Architecture

- **Customization Engine**: Uses the standard Windows Shell Class Info pattern (`desktop.ini` with `IconResource` pointing to high-resolution multi-size `.ico` assets).
- **Directory Attributes**: Applies `FILE_ATTRIBUTE_READONLY` (`attrib +r`) to the folder—the exact signal required by Windows NT Explorer to parse `desktop.ini`.
- **Instant Shell Redraw**: Dispatches native Win32 `SHChangeNotify(SHCNE_UPDATEITEM)` and `SHCNE_ASSOCCHANGED` via P/Invoke to instantly refresh File Explorer without restarting `explorer.exe`.
- **Zero Elevated Privileges**: Runs 100% in user space (`HKEY_CURRENT_USER\Software\Classes\Directory\shell\FolderColorizer`); never requires Administrator/UAC prompts.
- **Safety Ledger**: Maintains `%LocalAppData%\FolderColorizer\data\assignments.json` to safely restore pre-existing folder configurations upon reset or uninstallation.

---

## 🚀 Installation & Uninstallation

### One-Click Installation (Zero Admin Rights)
Double-click `installer\install.bat`:
- Copies the binary and icon assets to `%LocalAppData%\FolderColorizer\`.
- Registers the cascading context menu in `HKCU`.
- Registers in Windows Settings &rarr; *Installed Apps* for standard uninstallation.

### Clean Uninstallation
Run `installer\uninstall.bat` or uninstall via Windows Settings:
- Asks if you wish to restore all colored folders to default Windows icons.
- Removes all registry entries and cleans up local icon files.

---

## 💻 CLI & GUI Usage

```powershell
# Apply a color to a folder
FolderColorizer.exe apply "D:\Projects\UrgentTask" red

# Reset a folder to default Windows icon
FolderColorizer.exe reset "D:\Projects\UrgentTask"

# Register context menu in HKCU
FolderColorizer.exe register

# Unregister context menu from HKCU
FolderColorizer.exe unregister

# View current colorized folder ledger
FolderColorizer.exe status

# Launch the interactive Palette Preview & Management GUI
FolderColorizer.exe gui
```

---

## 🧪 Automated Test Suite

Run `tests\run_tests.bat` or execute `tests\TestHarness.exe`:
- Basic color application & attribute verification
- All 6 colors verification
- Spaces & Unicode path support (`Folder with spaces & 漢字 日本語 🚀`)
- Non-destructive reset & user file preservation
- Color re-assignment / update
- Invalid color rejection
- Missing folder reporting
- Ledger audit trail persistence
- Registry context menu registration & cleanup
