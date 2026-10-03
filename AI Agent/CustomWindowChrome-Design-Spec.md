# Custom Window Chrome – Design-Spezifikation (MainWindow)

## Ziel
Ersetzung der Standard-Windows-Titelleiste des **MainWindow** durch eine moderne, dunkle Custom Title Bar im bestehenden GameChatBalancer-Stil.

## Scope (hart)
- Nur: `GameChatBalancer.Wpf/MainWindow.xaml` und `GameChatBalancer.Wpf/MainWindow.xaml.cs`
- Keine Änderungen an:
  - Audio-/Balance-Logik
n  - Arduino-/Hardware-Logik
  - App-Zuordnung/Erkennung
  - Settings-/Persistenz-/Startup-Logik
  - Overlay (Layout, Verhalten, Code)
  - Systray-Popup (Layout, Verhalten, Code)
  - Tray-Icon-/Hotkey-Logik

## Visuelle Richtung
- Basis: bestehende Dark/Blue-Fluent-Sprache
- Keine Neugestaltung des Content-Bereichs
- Keine zusätzlichen Dekorationseffekte

## Title-Bar-Layout
### Links
- Kleines App-Icon (GCB)
- Titel: `GameChatBalancer`
- Optional Untertitel nur, wenn Layout stabil bleibt

### Mitte
- Freier Drag-Bereich

### Rechts
- Button: Minimize
- Button: Maximize/Restore (zustandsabhängig)
- Button: Close

## Maße & Tokens
- Title-Bar-Höhe: 40–44 px
- Horizontales Padding: 12–16 px
- Icon: 16–20 px
- Window-Buttons: 44x32 px (Windows-ähnlicher Hit-Target)
- Abstände im 4px-Raster (4/8/12/16)

## Farb-/State-Definition
- Normal: `Brush.TextSecondary` / transparenter Hintergrund
- Hover (Min/Max): dezente Aufhellung (z. B. `#1AFFFFFF`)
- Hover (Close): klarer roter Zustand (z. B. `#C42B1C`)
- Pressed: jeweils dunklerer Zustand
- Disabled: reduzierte Opazität, lesbar

## Technische Umsetzung (WPF)
- `WindowStyle="None"`
- `ResizeMode="CanResize"`
- `AllowsTransparency="False"`
- `WindowChrome` (System.Windows.Shell):
  - `CaptionHeight` passend zur Title-Bar
  - `ResizeBorderThickness` aktiv
  - `CornerRadius=0`
  - `UseAeroCaptionButtons=False`

## Verhalten (muss erhalten bleiben)
- Drag Move über freie Title-Bar-Fläche
- Doppelklick auf Title-Bar: Maximize/Restore
- Minimize/Maximize/Restore/Close über Buttons
- Resize über alle Kanten
- Windows Snap/Snap Layouts wie möglich nativ erhalten
- Korrektes Verhalten in maximiertem Zustand (keine abgeschnittenen Inhalte)

## DPI/Scaling
- Keine fixed-pixel Hacks außerhalb Title-Bar
- WPF-Layout + bestehende Ressourcen
- Test bei 100% / 125% / 150%

## Regression-Schutz
Vor Abschluss prüfen:
1. MainWindow Start/Anzeige
2. Drag/Min/Max/Restore/Close
3. Resize + Snap
4. Overlay unverändert
5. Systray-Popup unverändert
6. Tray-Verhalten unverändert
7. Audio/Arduino/Settings unverändert
8. Build fehlerfrei

## Geplante neue UI-Elemente
- `TitleBarRoot` (Grid)
- `TitleBarDragRegion` (nicht-interaktiver Bereich)
- `BtnMinimize`, `BtnMaxRestore`, `BtnClose`
- Optional `TitleBarSubtitle` (wenn stabil)

## Nicht-Ziele
- Kein Redesign von Navigation, Cards, Slidern, Listen
- Keine Logik-Refaktorierung
- Keine neuen externen Dependencies
