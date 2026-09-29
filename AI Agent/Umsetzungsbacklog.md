# GameChatBalancer – Umsetzungsbacklog (Tracking)

> Dieses Dokument ist das zentrale Steuerungsartefakt für die Migration.
> Es dient gleichzeitig als Arbeits-Backlog, Fortschritts-Tracker und gemeinsamer Referenzpunkt.

## Umgesetzte Leitlinien aus dem Master Prompt (relevant, neu)
- **Priorität strikt:** Funktionalität → Stabilität → Architektur → UX → Visual Design → Cleanup
- **Kleine Phasen statt Big-Bang-Rewrite** mit Quality-Gates je Phase
- **Keine Funktionsänderung ohne explizite Freigabe** (stattdessen dokumentieren)
- **Windows-11/Fluent-Zielbild** mit konsistentem Theme-System (Light/Dark/System)
- **Accessibility + DPI/Responsive** als feste Abnahmekriterien

## Quality-Gates (verbindlich je Phase)
1. Build ausführen (Solution/Projekt)
2. Fehler beheben
3. Warnings prüfen und auflösen (keine pauschale Suppression)
4. Regression gegen `AI Agent/Abnahmecheckliste-Ist-Verhalten.md`
5. Ergebnisse im Fortschrittsprotokoll dokumentieren

## Definition of Done (Modernisierung)
- [ ] Build erfolgreich
- [ ] App startet stabil
- [ ] System-Tray inkl. Show/Hide/Exit korrekt
- [ ] Game-/Chat-Balance unverändert funktionsfähig
- [ ] Arduino-Erkennung/COM/Reconnect/Invert funktionsfähig
- [ ] Settings speichern/laden ohne Verlust
- [ ] WPF-UI wirkt konsistent modern (Windows 11 / Fluent)
- [ ] Theme: Light/Dark/System funktioniert konsistent
- [ ] Drag & Drop für App-Zuordnung funktioniert
- [ ] DPI/Scaling ok (125%/150%)
- [ ] Fenstergrößen ok (1280x720, 1920x1080, 2560x1440)
- [ ] Accessibility-Basis erfüllt (Keyboard/Focus/Kontrast/Tooltips)
- [ ] Keine unnötigen Dependencies hinzugefügt

## Status-Legende
- `☐` Offen
- `◐` In Arbeit
- `☑` Erledigt
- `⚠` Blockiert

## Prioritäts-Legende
- `P0` Kritisch / zuerst
- `P1` Hoch
- `P2` Mittel
- `P3` Niedrig

## Meilensteine
- **M1:** Baseline + .NET-Upgrade abgeschlossen
- **M2:** Architektur entkoppelt (Core/Application/Infrastructure)
- **M3:** WPF-UI funktionsgleich zur WinForms-App
- **M4:** Produktionsreife inkl. Stabilisierung
- **M5 (optional):** WinUI-3-Track bewertet/prototypisiert

## Leitplanke (Entscheidung)
- **Primärer Migrationspfad:** WPF mit Fluent UI
- **WinUI 3:** nur optionaler Folgepfad in späterer Projektphase

---

## EPIC 00 – Projektsteuerung & Qualität

| ID | Status | Prio | Aufgabe | Abhängigkeit | Ziel/Definition of Done |
|---|---|---:|---|---|---|
| 00.01 | ☑ | P0 | Test-/Abnahmecheckliste aus Ist-Verhalten erstellen | - | Checkliste deckt: Tray, COM-Auto, USB-Reconnect, NR, Assignments, Persistenz |
| 00.02 | ☐ | P0 | Logging/Diagnose-Standard definieren | 00.01 | Fehlerpfade für Serial, Audio, USB sind nachvollziehbar |
| 00.03 | ☐ | P1 | Release- und Rollback-Strategie dokumentieren | 00.01 | Klarer Fallback auf letzte stabile Version |
| 00.04 | ☐ | P1 | Potential-Improvement-Log einführen (ohne sofortige Umsetzung) | 00.01 | Verbesserungen nur dokumentiert, keine stillen Funktionsänderungen |

---

## EPIC 01 – Frühe Plattform-Migration auf aktuelles .NET

> **Vorgabe:** .NET-Migration in einer frühen Projektphase umsetzen.

| ID | Status | Prio | Aufgabe | Abhängigkeit | Ziel/Definition of Done |
|---|---|---:|---|---|---|
| 01.01 | ☑ | P0 | Zielversion festlegen (aktuelles LTS) | - | Ziel-TFM entschieden und dokumentiert |
| 01.02 | ☑ | P0 | Projekt auf neues .NET-TFM migrieren | 01.01 | Build erfolgreich auf neuem TFM |
| 01.03 | ☑ | P0 | Paketkompatibilität prüfen/aktualisieren | 01.02 | `System.IO.Ports`, `System.Management` kompatibel |
| 01.04 | ☑ | P0 | Smoke-Test Kernfunktionen auf neuem TFM | 01.02, 01.03 | Tray, Serial, Audio, Persistenz laufen unverändert |
| 01.05 | ☑ | P1 | CI/Build-Konfiguration auf neues .NET aktualisieren | 01.02 | Reproduzierbarer Build lokal + CI |

## EPIC 02 – Architektur-Entkopplung (ohne UI-Wechsel)

| ID | Status | Prio | Aufgabe | Abhängigkeit | Ziel/Definition of Done |
|---|---|---:|---|---|---|
| 02.01 | ☐ | P0 | Domain-Modelle und Mix-Regel isolieren | 01.04 | Mix-Berechnung als testbare Kernlogik |
| 02.02 | ☑ | P0 | `IAudioSessionService` + Implementierung kapseln | 02.01 | Audio-Interop nur noch über Interface |
| 02.03 | ☑ | P0 | `ISerialDeviceService` + Protokolladapter kapseln | 02.01 | `syn/ack/get/NR` in separater Service-Schicht |
| 02.04 | ☑ | P0 | `IUsbWatcherService` kapseln | 02.03 | Reconnect-Events UI-unabhängig |
| 02.05 | ☑ | P0 | `ISettingsStore` einführen | 02.01 | Persistenzzugriffe zentralisiert |
| 02.06 | ☑ | P1 | WinForms nur als Adapter auf neue Services | 02.02-02.05 | Bestehende UI funktional unverändert, aber entkoppelt |

## Aktueller Fokus (Startvorschlag)
1. `03.01` WPF-Projektgrundstruktur
2. `03.02` WPF ViewModels für Assignment/Hardware/Status
3. `03.04` WPF Tray-Integration
4. `03.05` WPF Hardware-Steuerung anbinden
5. `03.03` WPF Drag&Drop-Zuordnung
6. `03.06` Systray-/Lifecycle-Host von `Form1` entkoppeln (eigenen Host einführen), danach `Form1` vollständig entfernen

## Fortschrittsprotokoll

| Datum | Änderung | Verantwortlich |
|---|---|---|
| 2026-09-27 | 03.03 korrigiert: AudioSessionsChanged auf robustes CoreAudio-Session-Monitoring umgestellt (dedizierter MTA-Thread + sauberes Register/Unregister), damit SessionCreated-Events zuverlässig ankommen; Build erfolgreich | AI Agent |
| 2026-09-27 | 03.03 fortgeführt: Eventbasierte Auto-Aktualisierung für neu erscheinende Audio-Sessions ergänzt (CoreAudio `SessionCreated` statt Prozessstart-Trigger), inkl. sauberem Start/Stop/Unsubscribe für rückbaubare Änderung; Build erfolgreich | AI Agent |
| 2026-09-27 | Planung ergänzt: neuer Schritt `03.06` aufgenommen, um Systray/Lifecycle aus `Form1` zu lösen und `Form1` später löschen zu können | User + AI Agent |
| 2026-09-27 | 03.03 begonnen: WPF Assignment-Listen auf echte Daten umgestellt (Game/Chat/Verfügbar inkl. App-Icons), Drag&Drop zwischen Kategorien implementiert, Persistierung der Zuordnung zeitnah über `ISettingsStore.ScheduleSave()` verdrahtet, Build erfolgreich | AI Agent |
| 2026-09-27 | 03.05 fortgeführt: Hardware-Status modernisiert (Status-Dot, Noise Reduction, Invert-Toggle inkl. Arduino-Confirm/Apply/Error-Visualisierung), COM-Port-UI bewusst entfernt bei unveränderter Service-Logik, Build erfolgreich | AI Agent |
| 2026-09-27 | 03.02 fortgeführt: Assignment-Bereich auf 2-Ebenen-Layout umgestellt (oben Game/Chat, darunter volle Breite „Verfügbare Apps (nicht zugewiesen)“), Build erfolgreich | AI Agent |
| 2026-09-27 | 03.02 gestartet: WPF Basis-Shell + Sidebar-Navigation umgesetzt (`GameChatBalancer.Wpf`), Design-Token/Theme-Ressourcen in `App.xaml` angelegt, Build erfolgreich | AI Agent |
| 2026-09-27 | 03.01 abgeschlossen: WPF-Projekt `GameChatBalancer.Wpf` erstellt und in `AudioControl.sln` eingebunden, Ziel-TFM `net10.0-windows` verifiziert, Build erfolgreich | AI Agent |
| 2026-09-26 | 01.05 abgeschlossen: CI-Workflow hinzugefügt (`.github/workflows/ci.yml` mit Restore/Build/Test auf .NET 10), SDK-Pinning via `global.json` (`10.0.401`), lokale Validierung per Build/Test erfolgreich | AI Agent |
| 2026-09-26 | Adapter-Bereinigung nachgezogen: verbleibende direkte `USBandCOM`/`Properties.Settings`-Zugriffe in `Form1` (COM-Port-Handler) entfernt; Reconnect-Overhead reduziert (Reconnect-Guard + keine Deletion-Reconnects ohne aktive Verbindung), Build erfolgreich | AI Agent |
| 2026-09-26 | 02.06 abgeschlossen: `Form1` weiter als UI-Adapter geschnitten (Audio-Mix in `IAudioBalanceService`, verbleibende direkte `USBandCOM`/Settings-Zugriffe entfernt), Build erfolgreich | AI Agent |
| 2026-09-26 | 02.05 abgeschlossen: `ISettingsStore` + `SettingsStore` eingeführt, `Form1` auf Settings-Service umgestellt, hybride Persistenz (deferred + immediate) implementiert, SessionEnding-Fallback für Windows-Shutdown ergänzt, Build erfolgreich | AI Agent |
| 2026-09-26 | 02.02 abgeschlossen: `IAudioSessionService` + `AudioSessionService` eingeführt, `Form1` nutzt Audio-Service statt direkter `AudioManager`-Aufrufe, Build erfolgreich | AI Agent |
| 2026-09-26 | 02.04 abgeschlossen: `IUsbWatcherService` + `UsbWatcherService` eingeführt, USB-Watcher Start/Stop in `Form1` entkoppelt, Build erfolgreich | AI Agent |
| 2026-09-26 | 02.03 abgeschlossen: `ISerialDeviceService` + `SerialDeviceService` eingeführt, `Form1` nutzt Serial-Service statt direkter `USBandCOM`-Aufrufe | AI Agent |
| 2026-09-26 | 01.04 abgeschlossen: Retest nach Shutdown-Fix erfolgreich (inkl. Debug-Stop ohne Fehler), Build weiterhin erfolgreich | User + AI Agent |
| 2026-09-26 | 01.04 gestartet: Smoke-Test-Dokument angelegt (`AI Agent/Smoke-Test-01.04-net10.md`), Build-Teil bestanden, manuelle Hardware-/UI-Validierung ausstehend | AI Agent |
| 2026-09-26 | Plan um Master-Prompt-Leitlinien ergänzt (Quality-Gates, DoD, Accessibility, Responsive/DPI, Potential-Improvement-Log) | User + AI Agent |
| 2026-09-25 | 01.03 abgeschlossen: `System.IO.Ports` und `System.Management` auf `10.0.12` aktualisiert, Build erfolgreich | AI Agent |
| 2026-09-25 | 01.02 abgeschlossen: TFM auf `net10.0-windows` angehoben, Build erfolgreich; WinForms-Warnungen (WFO1000) in `Form1` behoben | AI Agent |
| 2026-09-25 | 01.01 abgeschlossen: Zielversion auf `net10.0-windows` festgelegt | User + AI Agent |
| 2026-09-25 | 00.01 abgeschlossen: Abnahmecheckliste erstellt (`AI Agent/Abnahmecheckliste-Ist-Verhalten.md`) | AI Agent |
| 2026-09-25 | Konkreter Startplan ergänzt: .NET zuerst, danach Entkopplung von USBandCOM/AudioManager | User + AI Agent |
| 2026-09-25 | UI-Strategie konkretisiert: WPF als Primärpfad akzeptiert, WinUI 3 auf später verschoben | User + AI Agent |
| YYYY-MM-DD | Backlog initial erstellt | AI Agent |
