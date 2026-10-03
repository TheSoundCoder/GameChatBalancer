# GameChatBalancer – Mini-Design-Spec Systray-Popup (WPF, Fluent)

## Ziel
Systray-Popup visuell an die neue WPF-Hauptoberfläche angleichen (Windows-11-/Fluent-inspiriert), mit zwei klaren Zuständen:
1. **Arduino verbunden** (Hardware-Steuerung aktiv)
2. **Arduino nicht verbunden** (Software-Steuerung aktiv)

Die Mockups sind Leitlinie, keine pixelgenaue Vorgabe. Funktionalität/Stabilität haben Vorrang.

---

## 1) Layout & Größe
- Popup als kompakte Card über der Taskleiste.
- Zielgröße (Startwert): ca. **360–400px Breite**, Höhe dynamisch nach Inhalt.
- Außenabstand: 8–12px zur Taskleistenkante.
- Innenabstand Card: 16px.
- Vertikaler Rhythmus: 8/12px zwischen Gruppen.

### Struktur (von oben nach unten)
1. **Header**
   - App-Icon + Titel "GameChatBalancer"
2. **Status-Card**
   - Dot + Statuszeile + optionale Geräteinfo
3. **Balance-Bereich**
   - Game/Chat Werte + Gradient-Track + Indikator
4. **Mode-spezifischer Block**
   - Connected: Noise Reduction + Invert
   - Disconnected: Software-Control-Hinweis + Hotkeys
5. **Actions-Liste**
   - Open App
   - Start with Windows (Toggle)
   - Exit

---

## 2) Farb- & Materialsystem
(auf Basis `AI Agent/UI-Design-Reference.md`)

- Background/Base: `#0A1326`
- Surface 1: `#10203A`
- Surface 2: `#132743`
- Border/Subtle: `#2A3D5A`
- Text Primary: `#F2F6FF`
- Text Secondary: `#AFC0D8`
- Accent Game: `#2B8CFF`
- Accent Chat: `#8A5CFF`
- Success (Connected): `#22C55E`
- Error (Disconnected): `#EF4444`

Material:
- Hauptpanel: Surface 2
- Eingelassene Bereiche/Rows: Surface 1
- Border 1px subtil
- Keine harten Glows, nur sanfte Kontraste

---

## 3) Typografie
- Font: Segoe UI Variable / Segoe UI
- Größen:
  - Titel: 30
  - Abschnittstitel: 16
  - Primärtext: 14
  - Sekundärtext/Meta: 12–13
- Gewicht:
  - Titel/Sektion: Semibold/Bold
  - Meta: Regular

---

## 4) Corner Radius, Spacing, Controls
- Große Card: 12px
- Kleine Unterkarten/Rows: 8–10px
- Toggle-Stil identisch zum WPF-Hauptfenster (`HardwareToggleStyle`)
- Dropdown-Stil für Noise Reduction an Hauptfenster angleichen
- Fokuszustände sichtbar, Hover klar, Disabled lesbar

---

## 5) Zustand A – Arduino verbunden
- Statuszeile:
  - Grüner Dot + „Arduino verbunden“
  - Unterzeile optional: Gerät + Firmware
- Hardware-Steuerung sichtbar:
  - Noise Reduction (Value + Auswahl)
  - Invert Control (Toggle)
- Software-Control-Hinweis **ausgeblendet**

## 6) Zustand B – Arduino nicht verbunden
- Statuszeile:
  - Roter Dot + „Kein Arduino verbunden“
  - Unterzeile: „Software-Steuerung aktiv“
- Hardware-Steuerung **ausgeblendet**:
  - Kein Noise Reduction Block
  - Kein Invert Toggle
- Software-Control-Info sichtbar:
  - Hinweistext zu 5%-Schritten
  - Hotkeys:
	- Ctrl + Shift + ← / →
	- Ctrl + Shift + ↓ (Center 50)

---

## 7) Interaktion
- Open App: öffnet Hauptfenster
- Start with Windows: synchron mit bestehender Autostart-Logik
- Exit: beendet App sauber
- Klick außerhalb Popup: schließt Popup (wie Kontext-ähnliches Flyout)

---

## 8) Accessibility & UX
- Tastaturfokus für alle aktiven Elemente
- Nicht nur Farbe als Statussignal (Text immer vorhanden)
- Kontrast min. AA-nah für Primärtexte
- Tooltips für Status/unklare Icons

---

## 9) Technische Leitplanken für Umsetzung
- Schrittweise umsetzen:
  1. Nur Popup-Layout + Styles
  2. State-Switch Connected/Disconnected
  3. Actions/Bindings
- Bestehende Services/Logik zuerst weiterverwenden, keine Big-Bang-Refactorings.
- Nach jedem Schritt: Build + kurzer Smoke-Test.

---

## 10) Abnahmekriterien (Systray-Phase)
- Popup folgt visuell der WPF-Designsprache.
- Connected/Disconnected unterscheiden sich klar und korrekt.
- Bei Disconnected sind Hardware-Controls vollständig ausgeblendet.
- Software-Control-Hotkeys sind im Popup sichtbar dokumentiert.
- Actions (Open App, Start with Windows, Exit) funktionieren wie bisher.
- Keine Regression bei Tray-Show/Hide/Exit.
