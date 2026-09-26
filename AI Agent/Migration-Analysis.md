# GameChatBalancer – Architektur- und Migrationsanalyse

## Scope
- Repository vollständig analysiert (Code + Dokumentation)
- Keine Codeänderungen vorgenommen
- Ziel: UI-Migration zu nativer Windows-11-Erfahrung bei Erhalt der bestehenden Hardware-/Protokoll-Funktionalität

---

## 1) Architekturüberblick (Ist-Zustand)

### Solution/Projekte
- **Solution:** `AudioControl.sln`
- **Projekt:** `AudioControl/AudioControl.csproj` (Single-Project-Desktop-App)

### Anwendungstyp
- **Desktop-App mit Windows Forms** (Systray-zentriert)
- Einstiegspunkt: `Program.cs` → `Application.Run(new Form1())`

### Schichten (implizit)
1. **UI-Schicht (WinForms):** `Form1.cs` + `Form1.Designer.cs`
2. **Hardware/Serial-Schicht:** `USBandCOM.cs`
3. **Windows Audio Integration:** `AudioManager.cs` (CoreAudio COM Interop)
4. **Konfiguration:** `Properties.Settings` / `App.config`

### Laufzeit-/Lifecycle-Modell
- Startet minimiert und lebt primär im **System Tray**
- GUI wird über Tray-Menü „Show“ geöffnet
- Bei Minimize wird Form versteckt (`Hide`) und Tray bleibt aktiv
- Beim Exit:
  - User-Settings speichern (`GAME`, `CHAT`, `ComPort`, `NoiseReduction`, `Invert`)
  - COM-Port schließen

---

## 2) Aktuelle UI-Technologie

- **Windows Forms** (`UseWindowsForms=true`)
- Klassisches Designer-basiertes UI
- Drag&Drop zwischen ListBoxen für App-Zuordnung
- `NotifyIcon` + `ContextMenuStrip` für Tray-Bedienung

---

## 3) .NET-Version

- `TargetFramework`: **`net7.0-windows`**
- `Nullable`: aktiviert
- `ImplicitUsings`: aktiviert

Hinweis: .NET 7 ist außerhalb regulärem Supportfenster. Für Windows-11-Zielbild ist ein Upgrade auf aktuelles LTS sinnvoll (z. B. .NET 8/10 je nach Zielzeitpunkt).

---

## 4) System-Tray-Implementierung

- WinForms `NotifyIcon` (`trayicon`) mit Kontextmenü
- Menü enthält u. a.:
  - COM-Status (`systrayCom`)
  - letzter Wert (`systrayVolume`)
  - Show/Exit
- Startzustand minimiert, Taskbar versteckt (`ShowInTaskbar=false`)

---

## 5) Arduino-/Serial-Kommunikation

### Transport
- `System.IO.Ports.SerialPort`
- Baudrate: **115200**
- Event-basiertes Lesen über `DataReceived`

### Auto-Detect/Handshake
- Bei `ComPort=Auto` werden verfügbare Ports iteriert
- Handshake:
  - App sendet: `syn`
  - Erwartete Antwort: enthält `ack`

### Laufzeitprotokoll (aus Code + Doku)
- App → Device:
  - `get` (aktuellen Wert anfordern)
  - `NR=0..3` (Noise Reduction setzen)
- Device → App:
  - numerische Werte `0..100` (Volume-Mix-Position)
  - Bestätigung `NR=`/NR-Response

### Robustheit
- USB-Watcher via WMI (`ManagementEventWatcher` auf `Win32_USBHub`)
- Bei Connect/Disconnect wird reconnect/init ausgelöst

---

## 6) Windows-Audio-Integration

- Eigene CoreAudio-COM-Interop in `AudioManager.cs`
- Session-Enumeration über `IAudioSessionManager2`
- Prozess-basierte Volume-Steuerung per Prozessname
- Logik:
  - Wert < 50: GAME runter (0..100 skaliert), CHAT 100
  - Wert > 50: CHAT runter, GAME 100
  - Wert = 50: beide 100
- Mehrere Apps je Kategorie via CSV-Listen (`GAME`, `CHAT`)

---

## 7) Konfigurationspersistenz

- `Properties.Settings` (User Scope)
- Persistierte Schlüssel:
  - `GAME`
  - `CHAT`
  - `ComPort`
  - `NoiseReduction`
  - `Invert`
- Speicherung bei Exit und bei bestimmten UI-Änderungen (z. B. COM-Auswahl)

---

## 8) Application Detection / Assignment

- Erkennung aktiver Audio-Apps über CoreAudio Session Enumeration
- Anzeige in `lb_AudioProcesses`
- Zuordnung per Drag&Drop in `lb_GAME` / `lb_CHAT`
- Speicherung als comma-separated Strings

---

## 9) Abhängigkeiten

### NuGet
- `System.IO.Ports` 7.0.0
- `System.Management` 7.0.1

### Plattform-/API-Abhängigkeiten
- Win32/CoreAudio COM
- WMI-Events (USB Detection)
- Serial Port
- Windows-spezifischer Desktop-Stack

---

## 10) Bewertung der Ziel-UI-Optionen

## Option 1: WinUI 3 / Windows App SDK

### Vorteile
- Modernste native Windows-11 UI
- Fluent Design out-of-the-box
- Gute langfristige UI-Ausrichtung für Windows

### Risiken / Aufwand
1. **System Tray nicht nativ first-class** in WinUI 3
   - Tray benötigt Win32-Interop/zusätzliche Komponenten
2. **Drag&Drop- und Desktop-Verhaltensmigration** erfordert UI-Neubau
3. **Lifecycle (minimized-to-tray)** muss explizit neu implementiert werden
4. **Mehr Moving Parts** (WinUI Windowing + Interop + ggf. Packaging/Unpackaged-Modell)
5. Höheres Migrationsrisiko bei gleichzeitigem Erhalt aller Edge-Cases

### Eignung für dieses Repo
- Gut für „finales“ Windows-11-Frontend
- Höheres initiales Risiko für ein kleines, hardware-nahes Tool mit Tray-Fokus

---

## Option 2: WPF mit modernem Fluent UI

### Vorteile
1. **Reif/stabil für Desktop + Tray-Szenarien**
2. Sehr gute Interop-Fähigkeit mit bestehender Windows-/COM-/Serial-Logik
3. Einfachere schrittweise Migration (ViewModel + Services)
4. Fluent-Look via Libraries (z. B. Wpf.Ui/ModernWpf/MahApps)
5. Geringeres Risiko bei Funktionsparität

### Risiken / Aufwand
- UI wird ebenfalls neu aufgebaut (XAML statt Designer)
- Tray typischerweise über `TaskbarIcon`-Library/WinForms-Interop
- Kein „WinUI-native“ Stack, obwohl visuell sehr nah an Windows 11 möglich

### Eignung für dieses Repo
- Sehr gut für **inkrementelle, risikoarme Modernisierung**
- Besonders passend wegen starkem Tray-/Hardware-/Interop-Fokus

---

## 11) Migrationrisiken (gesamt)

1. **Threading/Dispatching:** Serial + USB Events aktualisieren UI; bei neuer UI-Architektur sauber über Dispatcher marshallen.
2. **CoreAudio-Interop-Stabilität:** COM-Objekte korrekt freigeben, Session-Enumeration robust halten.
3. **Tray-Parität:** Show/Hide, Statusanzeigen, Exit-Verhalten exakt erhalten.
4. **Prozessname-Matching:** aktuelle Substring-Logik kann Fehlzuordnungen verursachen; bei Refactoring Verhalten testen.
5. **Konfigurationskompatibilität:** bestehende Settings-Werte migrieren ohne Datenverlust.
6. **Hardware-Protokoll-Kompatibilität:** `syn/ack/get/NR=*` unverändert beibehalten.
7. **Reconnect-Logik:** USB Disconnect/Connect muss weiterhin autonom funktionieren.

---

## 12) Empfohlene Zielarchitektur

**Empfehlung:**
- **Phase 1:** Architektur entkoppeln + WPF Fluent UI als neue Shell
- **Phase 2 (optional):** WinUI-3-Shell als zweite Frontend-Option auf derselben Core-Logik

### Zielprinzipien
- UI von Hardware-/Audio-Logik trennen
- Domain-Services testbar machen
- Protokoll und Business Rules unverändert halten

### Schichten
1. **Domain/Core**
   - Mix-Algorithmus
   - Kategorien (Game/Chat), Assignment-Modell
2. **Infrastructure**
   - `ISerialDeviceService`
   - `IAudioSessionService`
   - `IUsbWatcherService`
   - `ISettingsStore`
3. **Application Layer**
   - Orchestrierung, Commands, State Machine
4. **Presentation (WPF/WinUI)**
   - Views + ViewModels
   - Tray Adapter

---

## 13) Vorschlag Projektstruktur

```text
GameChatBalancer.sln
src/
  GameChatBalancer.Core/
	Models/
	Services/
	Rules/
  GameChatBalancer.Infrastructure.Windows/
	Audio/
	  CoreAudioInterop.cs
	  AudioSessionService.cs
	Serial/
	  SerialDeviceService.cs
	  ArduinoProtocol.cs
	Usb/
	  UsbWatcherService.cs
	Settings/
	  UserSettingsStore.cs
  GameChatBalancer.Application/
	UseCases/
	State/
	Contracts/
  GameChatBalancer.UI.Wpf/
	App.xaml
	Views/
	ViewModels/
	Tray/
  GameChatBalancer.UI.WinUI/   (optional, später)
	...
tests/
  GameChatBalancer.Core.Tests/
  GameChatBalancer.Application.Tests/
```

---

## 14) Inkrementeller Migrationsplan

## Phase 0 – Baseline absichern
1. Repro-Build + Laufzeit-Check dokumentieren
2. Protokollverhalten (`syn/ack/get/NR`) und Reconnect-Szenarien als Referenz festhalten
3. Kernflows als manuelle Test-Checkliste erfassen

## Phase 1 – Entkopplung im bestehenden Repo
1. Audio- und Serial-Logik in Services extrahieren (ohne Verhaltensänderung)
2. Settings-Zugriff kapseln
3. UI ruft nur noch Service-Interfaces auf

## Phase 2 – Neues WPF-Frontend (empfohlen)
1. Neues WPF-Projekt hinzufügen
2. ViewModels + Binding für:
   - App-Listen
   - Zuordnung Game/Chat
   - COM/NR/Invert
   - Status/Debug
3. Tray-Host implementieren (Show/Exit/Status)
4. Core-Services wiederverwenden

## Phase 3 – Paritätsvalidierung
1. Funktionale Vergleichstests WinForms vs WPF
2. Hardware Disconnect/Reconnect testen
3. Audio-Mix-Verhalten in Edge-Cases prüfen (50, Richtungswechsel, mehrere Prozesse)

## Phase 4 – Ablösung WinForms
1. WPF als primäres Deployable
2. WinForms nur noch Fallback (kurzfristig)
3. Nach Stabilitätsphase WinForms entfernen

## Phase 5 – Optionaler WinUI-3-Track
1. Auf derselben Core/Application-Schicht WinUI-UI prototypisieren
2. Tray/Lifecycle über Interop absichern
3. Nur übernehmen, wenn kein Funktionsverlust gegenüber WPF

---

## 15) Klare Empfehlung

Für dieses konkrete Repository ist **WPF + modernes Fluent UI** der beste erste Zielpfad:
- niedrigstes Risiko bei vollständigem Funktionserhalt
- beste Kontrolle über Tray-/Interop-/Hardware-Spezifika
- dennoch native Windows-11-Optik erreichbar

**WinUI 3** bleibt eine gute strategische Option als zweiter Schritt, sobald die Logik sauber entkoppelt ist.
