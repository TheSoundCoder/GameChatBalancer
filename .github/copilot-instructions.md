# Copilot Instructions

## General Guidelines
- The display language of the application shall be English and must be documented as a design guideline.

## Projektrichtlinien
- Der Nutzer möchte ein zentrales Umsetzungsbacklog und eine Designentscheidungsdokumentation als Steuerungs- und Gedächtnisartefakte im Repository; außerdem soll die Migration auf eine aktuelle .NET-Version (net10.0-windows) bereits in einer frühen Projektphase eingeplant werden.
- Der Nutzer möchte auf WPF mit Fluent UI migrieren und eine WinUI-3-Migration höchstens zu einem späteren Zeitpunkt evaluieren. Für die Modernisierung sollen Funktionalität und Stabilität strikt vor UX/Visuals priorisiert werden; Migration in kleinen Phasen mit Build/Warning/Test-Gates, WPF+Fluent als Ziel, WinUI 3 nur später optional.
- Der Nutzer bevorzugt Branch-Workflow: erst refactor/services-decoupling als Basisbranch, danach separater WPF-Upgrade-Branch darauf.
- Der Nutzer möchte den Migrationsplan um einen expliziten Schritt erweitern, der den Systray-/Lifecycle-Host von Form1 entkoppelt, damit Form1 später vollständig entfernt werden kann.
- Änderungen sollen sorgfältig geplant und stabil umgesetzt werden, damit nichts kaputt geht.
- Umsetzung soll in kleinen Schritten erfolgen.

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
- Titel und Untertitel sollen direkt auf der Titelgrafik platziert werden, nicht oberhalb des Bildbereichs. Der Titel im Hero-Bild soll leicht kleiner und weiter links/unten positioniert werden; er soll mit sehr dezentem Schatten/Glow für bessere Lesbarkeit auf hellen Bildbereichen versehen werden.
- Die bestehende GameChatBalancer-Funktionalität muss erhalten bleiben und in die neue Designsprache übertragen werden.
- Der Schriftzug "GameChatBalancer" soll in der WPF-Shell im zentralen Hauptelement erscheinen (nicht primär in der Sidebar).
- Im zentralen WPF-Bereich soll Chat links und Game rechts angezeigt werden.
- Wenn keine Hardware-Verbindung besteht, soll der Status in der WPF-UI explizit als "Disconnected" angezeigt werden (statt "Connected").
- Im Branch Software-Control soll bei fehlender Arduino-Verbindung in der WPF-UI statt Noise Reduction/Invert Control eine deutliche Software-Control-Info mit Shortcut-Hinweisen gemäß Mockup angezeigt werden; erster Schritt: nur Umschaltung der Anzeige bei disconnected.
- Der Titelbild im oberen Produkt-Card-Bereich soll den Kasten möglichst vollständig ausfüllen, ohne innere Ränder.
- Für den Hardware-Status oben rechts in der WPF-UI bevorzugt der Nutzer eine reine farbige Kreis-Anzeige ohne Text im Badge; der Hardware-Status-Dot soll einen klaren, scharfen Rand ohne unscharfen/ausgefransten Effekt haben.
- Für den Systray-Redesign soll die neue Systray-UI die bestehende WPF/GUI-Optik aus den Mockups weitgehend übernehmen (Connected/Disconnected Varianten). Open App/Start with Windows/Exit sollen eher als flache Schaltflächen mit horizontalen Trennlinien statt card-artiger Button-Container gestaltet werden.
- Bei Design-Anpassungen sollen Chat/Game-Icon-Änderungen im Hauptfenster (MainWindow) erfolgen, nicht nur im Systray-Popup.

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
Vor Umsetzung einer UI-Phase muss eine kurze Design-Spezifikation vorliegen (Palette, Typografie, Spacing, Radius, Controls, Cards, Navigation, Icons, States, Light-Theme-Äquivalent, Layoutstruktur). Dafür ist die Datei `AI Agent/UI-Design-Reference.md` maßgeblich.

### Navigation
- Für die WPF-Navigation bevorzugt der Nutzer nur die Bereiche Home, Settings und About; separate Navigationseinträge für Game/Chat sollen entfallen, da die Zuordnung zentral im Hauptbereich erfolgt.
- Der Nutzer bevorzugt für den WPF-Hauptbereich das 2-Ebenen-Layout (oben Game/Chat, darunter Verfügbare Apps) und bewertet diese Variante als besser als eine dritte Spalte im rechten Utility-Bereich.
- Für die Sidebar-Navigation sollen Mockup-nahe Symbole je Eintrag verwendet werden und die aktive Selektion klar mit einem blauen vertikalen Streifen markiert sein. Die Button-Optik soll stärker am Mockup orientiert sein: bessere Lesbarkeit, größerer linker Innenabstand und klareres Highlighting der aktiven Option.

### UI-Layout
- Der zentrale Headerbereich soll in zwei getrennte Cards aufgeteilt werden: eine eigene Product-Branding-Fläche (später durch Grafik ersetzt) und eine separate "Audio Balance"-Card.
- Im Audio-Balance-Bereich sollen 'Game' und 'Chat' optisch größer hervorgehoben werden; die Prozentwerte sollen darunter stehen, um dem ursprünglichen Mockup näher zu kommen.

## Service-Änderungen
- Änderungen an Services sollen nach Möglichkeit zunächst vermieden werden; wenn Service-Änderungen nötig sind, müssen sie vorab begründet und vom Nutzer bestätigt werden. Vor Änderungen an service-naher Logik (z. B. AudioManager) immer vorab kurz begründen und erst nach expliziter Nutzerbestätigung umsetzen.
- Bei neuen Service-nahen Änderungen soll die Umsetzung so erfolgen, dass sie im Zweifel sauber rückbaubar ist.
- Interface-Namen sollen kein unnötiges 'Audio'-Präfix tragen; IAudioDiagnosticsSink zu generischem Namen ohne Audio umbenennen.

## Debugging
- Der Debug-Bereich soll scrollbar sein; neue Debug-Logeinträge sollen oben angezeigt/eingefügt werden (neueste zuerst).
- Die Debug-Option soll nicht mehr vom Programmstart abhängen, sondern davon, ob beim Klick auf Systray->Show die Shift-Taste gehalten wird.

## Software-Control
- Für Software-Control sollen Hotkeys gelten: Ctrl+Shift+Left verschiebt Balance Richtung Chat, Ctrl+Shift+Right Richtung Game; dabei gilt der Arduino-Invert-Status nicht. 
- Software-Control-Hotkeys sollen nur bei Arduino-Disconnected registriert sein und bei Connected vollständig deaktiviert werden, um Overhead zu minimieren. Im Disconnected-Zustand sind die Hotkeys erst nach Öffnen des MainWindows aktiv.
- Beim ersten Schritt auf das 5%-Raster gesnappt werden (z.B. 42 -> Decrease 40, Increase 45), danach in 5%-Schritten weiter.

## MainWindow Anpassungen
- Ab jetzt keine ungefragten Änderungen im MainWindow vornehmen; MainWindow nur nach expliziter Anforderung anpassen.

## Paritäts-Checkliste
- Bei Punkt 7 (Arduino-Verbindung, Live-Wechsel auf Connected) hängt sich die Anwendung aktuell auf.
- In der aktuellen UI wird der COM-Port im Systray nicht mehr angezeigt; Testpunkt 9 ist daher N/A.