# 🛠️ Pillar 2: Technical Requirements Document (TRD) — Folder Colorizer for Windows 11

## 1. Technical Stack & Architecture
- **Core Platform**: Windows 11 x64 (Native C# .NET compiled into an independent, self-contained executable).
- **Compilation Toolchain**: Native Windows C# compiler (`csc.exe` v4.0.30319 / .NET runtime), guaranteeing zero third-party framework installation requirements.
- **Shell Customization Mechanism**: 
  - Standard Windows Shell Class Info pattern using `desktop.ini`.
  - Windows API attribute flags (`SetFileAttributesW`):
    - Folder: `FILE_ATTRIBUTE_READONLY` (0x01) — mandatory signal to Windows Explorer to parse `desktop.ini`.
    - Configuration file: `FILE_ATTRIBUTE_HIDDEN | FILE_ATTRIBUTE_SYSTEM` (0x06).
- **Explorer Cache Invalidation**:
  - P/Invoke `shell32.dll` -> `SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW, folderPath, IntPtr.Zero)`
  - P/Invoke `shell32.dll` -> `SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero)`
- **Context Menu Integration**:
  - Per-user registry registration under `HKEY_CURRENT_USER\Software\Classes\Directory\shell\FolderColorizer`.
  - Submenu cascading verbs registered via `SubCommands` or shell subkeys.
  - Zero administrator permissions (UAC) required for installation, registration, or routine color operations.
- **Asset Storage**:
  - `%LocalAppData%\FolderColorizer\icons\` stores 6 master multi-resolution `.ico` assets.
  - `%LocalAppData%\FolderColorizer\data\assignments.json` stores the per-user recovery and uninstallation ledger.

---

## 2. Strict Performance & Resource Budgets
| Metric | Budget Target | Rationale |
|---|---|---|
| **Apply Latency** | **< 100 ms** | Creating `desktop.ini`, setting attributes, and dispatching `SHChangeNotify` must be instantaneous. |
| **Explorer Visual Redraw** | **< 500 ms** | Folder icon in active Explorer window must update immediately without delay. |
| **Idle RAM Footprint** | **0 MB** | Zero background processes; executes strictly on-demand when user clicks context menu. |
| **Execution RAM** | **< 15 MB** | Lightweight CLI/worker execution lifecycle (< 0.2s duration). |
| **Disk Footprint** | **< 5 MB** | Total installed footprint including all 6 multi-size `.ico` files and binary. |

---

## 3. Mandatory File Tree Contract
```
FolderColorizer/
├── specs/
│   ├── 01_PRD.md
│   ├── 02_TRD.md
│   ├── 03_APP_FLOW.md
│   ├── 04_UI_UX.md
│   ├── 05_SCHEMA.md
│   └── 06_IMPLEMENTATION_PLAN.md
├── src/
│   ├── Core/
│   │   ├── ShellHelper.cs        # Win32 P/Invoke: SHChangeNotify, SetFileAttributes
│   │   ├── FolderManager.cs      # desktop.ini generator, attribute manager, reset logic
│   │   ├── LedgerManager.cs      # JSON assignment state tracker for undo/uninstall
│   │   └── RegistryManager.cs    # HKCU context-menu registration and removal
│   ├── Assets/
│   │   ├── folder_red.ico
│   │   ├── folder_blue.ico
│   │   ├── folder_green.ico
│   │   ├── folder_yellow.ico
│   │   ├── folder_orange.ico
│   │   └── folder_purple.ico
│   ├── Program.cs                # Entry point, CLI router (apply, reset, register, unregister)
│   └── build.bat                 # One-click native csc compilation script
├── tests/
│   ├── TestHarness.cs            # Automated verification test suite
│   └── run_tests.bat             # Test runner executing all P0 test cases
├── installer/
│   ├── install.bat               # Per-user setup: copies assets, registers HKCU shell
│   └── uninstall.bat             # Clean uninstaller: prompts for reset, cleans registry & assets
└── README.md                     # User documentation, keyboard shortcuts, limitations
```

---

## 4. Technical Constraints & Edge-Case Handling
1. **Spaces & Unicode Paths**: All CLI arguments and Win32 P/Invoke signatures utilize wide characters (`Unicode` / `CharSet.Unicode`) with robust quotation wrapping.
2. **Access Denied**: Folders inside `C:\Windows`, `C:\Program Files`, or protected ACLs return non-zero exit codes with descriptive human-readable diagnostics and never corrupt folder state.
3. **Network Drives & Substituted Drives**: Detects UNC or FAT32/exFAT drives where folder attributes or custom icons may behave differently, reporting appropriate warnings.
4. **Existing Customizations**: If a folder already contains a custom `desktop.ini`, FolderColorizer backs up the prior configuration in its ledger so a reset restores the previous custom state rather than clobbering it blindly.
