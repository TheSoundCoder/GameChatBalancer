# Copilot Instructions

## Projektrichtlinien
- Der Nutzer möchte ein zentrales Umsetzungsbacklog und eine Designentscheidungsdokumentation als Steuerungs- und Gedächtnisartefakte im Repository; außerdem soll die Migration auf eine aktuelle .NET-Version (net10.0-windows) bereits in einer frühen Projektphase eingeplant werden.
- Der Nutzer möchte auf WPF mit Fluent UI migrieren und eine WinUI-3-Migration höchstens zu einem späteren Zeitpunkt evaluieren. Für die Modernisierung sollen Funktionalität und Stabilität strikt vor UX/Visuals priorisiert werden; Migration in kleinen Phasen mit Build/Warning/Test-Gates, WPF+Fluent als Ziel, WinUI 3 nur später optional.
- Der Nutzer bevorzugt Branch-Workflow: erst refactor/services-decoupling als Basisbranch, danach separater WPF-Upgrade-Branch darauf.

## Architekturelles Prinzip (verbindlich)
### Event Driven
- GameChatBalancer bleibt event-driven.
- Keine Polling-Loops oder periodischen Timer für Funktionen, die eventbasiert getrieben werden können, insbesondere kein kontinuierliches Polling von Arduino-Status, Balance-Wert, UI-Status oder Overlay-Status.
- Das bestehende Verhalten bleibt erhalten: Arduino sendet nur bei Wertänderung.
- Statusänderungen sollen über Events oder äquivalente reaktive/eventgetriebene Mechanismen propagiert werden.
- Timer sind nur dort zulässig, wo sie inhärent sinnvoll sind (z. B. Overlay-AutoHide, Animationstiming, Debouncing).
- Event-driven Verhalten darf nicht durch periodisches Polling ersetzt werden.

## UI-Referenzdesign (WPF)
- Verbindliche Referenzdatei: `AI Agent/UI-Design-Reference.md`
- Referenzbildpfad im Repository: `assets/design-reference/GameChatBalancer-Reference.jpeg`
- Das Referenzbild ist eine visuelle Leitlinie, keine pixelgenaue Spezifikation.
- Die bestehende GameChatBalancer-Funktionalität muss erhalten bleiben und in die neue Designsprache übertragen werden.

### Verbindliche Gestaltungsregeln
- Windows-11-/Fluent-inspirierte visuelle Sprache
- Dark Modern UI mit Navy/Blau-Basis, subtilen Layern und klarer Hierarchie
- Akzentfarben: Blau (Game), Lila (Chat)
- Runde Ecken, moderne Cards/Panels, großzügige Abstände
- Moderne Sidebar-Navigation, zentrales Audio-Balance-Control, Hardware-Status, Quick Actions, System-Audio
- Theme-Unterstützung: Dark/Light/System
- Keine Emoji-Icons, stattdessen Fluent-/Windows-Style Icons
- Kein 1:1-Screenshot-Nachbau, kein RGB-Gaming-Look, keine unnötigen Effekte

### Vorgehenspflicht vor UI-Implementierung
Vor Umsetzung einer UI-Phase muss eine kurze Design-Spezifikation vorliegen (Palette, Typografie, Spacing, Radius, Controls, Cards, Navigation, Icons, States, Light-Theme-Äquivalent, Layoutstruktur). Dafür ist die Datei `AI Agent/UI-Design-Reference.md` maßgeblich.Vor Umsetzung einer UI-Phase muss eine kurze Design-Spezifikation vorliegen (Palette, Typografie, Spacing, Radius, Controls, Cards, Navigation, Icons, States, Light-Theme-Äquivalent, Layoutstruktur). Dafür ist die Datei `AI Agent/UI-Design-Reference.md` maßgeblich.