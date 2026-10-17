# 📋 Pillar 1: Product Requirements Document (PRD) — Folder Colorizer for Windows 11

## 1. Executive Summary & Value Proposition
**Folder Colorizer** is a lightweight, offline-first Windows 11 desktop utility that enables users to assign distinct, persistent colors to individual folders directly through the native File Explorer workflow. 

### Core Value Proposition
Color-code any folder in seconds, directly from File Explorer, making important directories, active client projects, finances, and personal archives visually recognizable at a glance without modifying folder contents, names, or file hierarchies.

---

## 2. Target Personas & Core Problems
- **Personal Productivity Users**: Managing deep folder hierarchies (Downloads, Documents, Photos, Taxes) that look identical under default Windows 11 icons.
- **Professionals & Freelancers**: Needing instant spatial/visual distinction between active projects, client deliverables, incoming assets, and archived work.
- **Students & Researchers**: Organizing course modules, lab data, thesis drafts, and urgent deadlines.
- **Core Pain Point**: Windows 11 presents virtually all user directories with identical yellow folder glyphs. Relying strictly on reading text labels introduces cognitive load, slows down navigation, and increases filing errors.
- **Promise**: Single-click color application that survives reboots, Explorer restarts, and folder relocations, backed by safe 1-click reset to default.

---

## 3. Product Principles
1. **One action, one result**: Applying or resetting a color takes no more than two clicks from File Explorer.
2. **Zero technical friction**: No PowerShell commands, command prompts, manual icon editing, or attribute manipulation required by the user.
3. **Strictly non-destructive**: Never delete, move, rename, touch, or modify any user files or child items within the colored folder.
4. **Persistent across lifecycle**: Icon customizations survive Explorer process restarts, Windows system reboots, and routine app updates.
5. **Completely reversible**: Restoring the default Windows folder icon is always available with 1-click.
6. **Honest compatibility**: Clear, non-technical warnings when folders are on unsupported/protected paths (read-only locations, network shares without metadata support, system folders).

---

## 4. Scope & Feature Boundaries

### MVP (v1.0) — Must Have
- **OS Support**: Windows 11 x64 (Build 22000+ through latest 24H2+ / Build 26300+).
- **Native Context Menu**: Right-click folder &rarr; `Folder Colorizer` submenu.
- **Six Curated Predefined Colors**:
  1. Red (`#DC2626`)
  2. Blue (`#2563EB`)
  3. Green (`#16A34A`)
  4. Yellow (`#EAB308`)
  5. Orange (`#EA580C`)
  6. Purple (`#9333EA`)
- **Reset to Default**: Instantly restores native Windows folder appearance and strips applied customization metadata.
- **Path Robustness**: Full support for spaces, Unicode/non-English characters, long paths (`MAX_PATH` handling), and quoted argument boundaries.
- **Metadata Ledger**: Local per-user registry/JSON ledger recording assignments for safe recovery, drift detection, and uninstallation.
- **Instant Shell Refresh**: Uses native Windows `SHChangeNotify` to redraw File Explorer without needing an Explorer restart.
- **Clean Uninstallation**: Restores all customized folders to default appearance or cleanly retains assets according to user choice, leaving zero broken icon references.
- **Offline & Private**: Zero internet connection required; zero telemetry, analytics, or background daemon resource drain.
- **Least Privilege**: Runs entirely under standard user privileges; no UAC / Administrator prompt needed for user-owned folders.

### Future Releases (v1.x / v2.0)
- Custom RGB color picker and custom palette imports.
- Multi-folder batch coloring selection.
- User-defined tag labels (e.g. Red = "Urgent", Blue = "Finance").
- Global keyboard shortcut palette.
- Assignment history and cloud/export configuration backup.
- Optional portable USB edition.

### Explicitly Out-of-Scope (Non-Goals)
- Recoloring the File Explorer window background, navigation pane, or ribbon.
- Recoloring individual file types, extensions, or drive letters.
- Hooking or patching Explorer system DLLs.
- Continuous background polling daemons or telemetry services.
