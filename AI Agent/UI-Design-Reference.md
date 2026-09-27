# GameChatBalancer – UI Design Reference (Windows 11 / Fluent)

## Zweck
Diese Datei ist die verbindliche Design-Referenz für die WPF-Modernisierung.
Sie beschreibt die visuelle Sprache und UX-Prinzipien auf Basis des bereitgestellten Referenz-Screenshots.

## Wichtige Regel
Das Referenzbild ist **keine pixelgenaue Spezifikation**.
Es dient als visuelle Richtung. Funktionalität und Stabilität bleiben priorisiert.

## Referenzbild
- Erwarteter Pfad im Repository: `assets/design-reference/GameChatBalancer-Reference.jpeg`
- Hinweis: Falls die Datei fehlt, bitte das bereitgestellte Referenzbild dort ablegen.

---

## Verbindliche Designprinzipien
- Windows-11-/Fluent-inspirierte Oberfläche
- Modernes, dunkles Desktop-UI mit klarer Hierarchie
- Tiefes Blau/Navy als Grundstimmung
- Subtile Layer/Transparenz, keine übertriebenen Effekte
- Runde Ecken, saubere Karten/Paneele
- Großzügige Abstände und moderne Typografie
- Akzentfarben: **Blau (Game)**, **Lila (Chat)**
- Moderne Sidebar-Navigation
- Zentrales großes Audio-Balance-Control
- Game-/Chat-Listen, Hardware-Status, Quick Actions, System-Audio
- Theme-Support: Dark/Light/System

## Verbote
- Kein 1:1-Nachbau des Screenshots
- Kein „Gaming RGB“-Look
- Keine unnötigen visuellen Effekte
- Keine Funktionseinbußen durch Visual-Refactoring

---

## Kurzspezifikation vor UI-Implementierung

### 1) Farbpalette (Startwerte)
- `Background/Base`: `#0A1326`
- `Surface 1`: `#10203A`
- `Surface 2`: `#132743`
- `Border/Subtle`: `#2A3D5A`
- `Text Primary`: `#F2F6FF`
- `Text Secondary`: `#AFC0D8`
- `Accent Game`: `#2B8CFF`
- `Accent Chat`: `#8A5CFF`
- `Success`: `#22C55E`
- `Warning`: `#F59E0B`
- `Error`: `#EF4444`

### 2) Typografie
- Primär: Segoe UI Variable / Segoe UI
- Größenraster: 12 / 14 / 16 / 20 / 28
- Gewichte: Regular, Semibold, Bold

### 3) Spacing-System
- Basis: 4px
- Nutzwerte: 4 / 8 / 12 / 16 / 24 / 32
- Karten-Innenabstand: 16–24px

### 4) Corner Radius
- Kleine Controls: 8px
- Cards/Panel: 12px
- Große Container: 16px

### 5) Control-Styles
- Klare Focus-Ringe
- Hover/Pressed/Disabled eindeutig
- Hoher Kontrast bei Text und Interaktion

### 6) Card-Styles
- Dunkle Layer-Flächen
- Subtile Border
- Sehr leichte Schattierung, keine harte Glow-Optik

### 7) Navigation
- Linke Sidebar mit klaren Sektionen
- Aktiver Eintrag deutlich markiert
- Home, Game, Chat, Settings, About

### 8) Icon-Style
- Fluent-/Windows-Style Icons
- Vektor/SVG bevorzugt
- Keine Emoji-Icons

### 9) Zustände
- Hover: leichte Aufhellung/Border-Akzent
- Pressed: dunkler, klare Rückmeldung
- Disabled: reduzierter Kontrast, aber lesbar

### 10) Light Theme (Äquivalent)
- Heller Hintergrund + dunkler Text
- Akzentfarben bleiben semantisch (Game=Blau, Chat=Lila)
- Gleiche visuelle Hierarchie wie Dark Theme

### 11) Layout-Struktur
- Links: Navigation
- Mitte: Header + Audio-Balance + App-Listen
- Rechts: Hardware-Status, Schnellaktionen, System-Audio
- Responsiv für 1280/1920/2560 + 125%/150% DPI

---

## Umsetzungsregeln
1. Bestehende Funktionalität zuerst verstehen und erhalten.
2. Funktionen in die neue Designsprache übertragen, nicht ersetzen.
3. UI muss auf unterschiedliche Fenstergrößen und DPI reagieren.
4. Accessibility mitdenken (Tastatur, Fokus, Kontrast, Tooltips, nicht nur Farbe).
5. Vor jeder größeren UI-Phase: Build + Smoke/Regression prüfen.
