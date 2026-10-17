# 🎨 Pillar 4: UI / UX Design Specification — Folder Colorizer for Windows 11

## 1. Visual Design & Aesthetic Philosophy
Folder Colorizer follows the **Windows 11 Fluent Design System**:
- Crisp, modern iconography with gentle corner rounding (`radius: 4px` on folder tabs, `6px` on folder flaps).
- Clean optical weight matching Windows 11 default `shell32.dll` and `imageres.dll` folder silhouettes.
- High contrast, easily distinguishable hues calibrated for both Light Mode and Dark Mode File Explorer themes.

---

## 2. Color Palette & Optical Specifications

| Color Name | Hex Code | RGB | Visual Role & Mental Association | Dark Mode Contrast Ratio |
|---|---|---|---|---|
| **Red** | `#DC2626` | `rgb(220, 38, 38)` | Urgent, Deadlines, Critical Bugs, Tax Filings | 5.8:1 |
| **Blue** | `#2563EB` | `rgb(37, 99, 235)` | Primary Projects, Active Work, Codebases | 6.2:1 |
| **Green** | `#16A34A` | `rgb(22, 163, 74)` | Completed, Approved, Finance, Invoices | 5.4:1 |
| **Yellow** | `#EAB308` | `rgb(234, 179, 8)` | In Review, Pending, Temporary Staging | 9.1:1 |
| **Orange** | `#EA580C` | `rgb(234, 88, 12)` | High Priority, Action Required, Marketing | 5.9:1 |
| **Purple** | `#9333EA` | `rgb(147, 51, 234)` | Personal, Design, Creative, Media Assets | 5.2:1 |

Each folder icon is rendered with:
- **Base/Back Plate**: 15% darker shade of the primary hex to simulate internal depth.
- **Front Flap**: Primary hex with a soft vertical gradient (+10% lightness at the top rim).
- **Edge Highlight**: 1px subtle optical rim for crisp separation on dark backgrounds.

---

## 3. Multi-Resolution Icon Asset Specifications
Every `.ico` file contains 7 mipmapped raster layers:
1. `16 × 16 px`: Details view, tree view in navigation pane (pixel-fitted, 1px borders).
2. `24 × 24 px`: Small icon view (125% DPI scale).
3. `32 × 32 px`: List view and standard tile view.
4. `48 × 48 px`: Medium icons view.
5. `64 × 64 px`: Large icons view (150% DPI scale).
6. `128 × 128 px`: Extra large view.
7. `256 × 256 px`: Ultra-high DPI / 4K monitors (lossless PNG compression within ICO container).

---

## 4. Context Menu Architecture

```
Folder Context Menu
│
├── ... (standard Windows Explorer verbs)
├── Folder Colorizer  >
│   │
│   ├── 🔴 Red
│   ├── 🔵 Blue
│   ├── 🟢 Green
│   ├── 🟡 Yellow
│   ├── 🟠 Orange
│   ├── 🟣 Purple
│   ├── ────────────────────── (separator)
│   └── ↺  Reset to Default
└── ...
```

---

## 5. Lightweight Palette Management GUI (P1 Optional / Diagnostic UI)
For users who prefer a desktop window over the context menu:
- Compact, borderless/acrylic dialog.
- Drag-and-drop target: *"Drop any folder here to change its color"*.
- 6 interactive color swatches with hover tooltips.
- Active assignments list with 1-click reset buttons.
