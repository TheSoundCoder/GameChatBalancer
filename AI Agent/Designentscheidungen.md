# GameChatBalancer – Designentscheidungen (ADR-Log)

> Dieses Dokument ist das zentrale Entscheidungsprotokoll.
> Jede relevante Architektur-/Technikentscheidung wird hier mit Kontext, Alternativen und Konsequenzen festgehalten.

## Verwendung
- Neue Entscheidung als neuer Abschnitt `ADR-XXXX`
- Status pflegen: `Proposed`, `Accepted`, `Superseded`, `Deprecated`
- Bei Änderungen nie alte Entscheidung löschen, sondern durch neue ADR ablösen

---

## ADR-0001 – Migrationsstrategie
- **Status:** Accepted
- **Datum:** 2026-09-25

### Kontext
Die bestehende App ist eine .NET-7-WinForms-Systray-App mit Hardware-Serial-Kommunikation und CoreAudio-Interop. Ziel ist eine native Windows-11-Anwendung ohne Funktionsverlust.

### Entscheidung
Migration **inkrementell** statt Big-Bang:
1. Frühzeitig auf aktuelles .NET migrieren
2. Architektur entkoppeln (Core/Application/Infrastructure)
3. Neue UI auf entkoppelter Schicht aufbauen

### Alternativen
- Direkter Komplettumstieg inkl. gleichzeitiger UI-/TFM-/Architektur-Neubau

### Konsequenzen
- Niedrigeres Risiko
- Bessere Testbarkeit
- Kürzere Feedbackzyklen

---

## ADR-0002 – Frühphase: .NET-Migration
- **Status:** Accepted
- **Datum:** 2026-09-25

### Kontext
Der aktuelle Stand ist `net7.0-windows`; langfristige Wartbarkeit und moderne Plattformfeatures erfordern eine aktuelle .NET-Version.

### Entscheidung
Die Migration auf ein **aktuelles .NET (LTS bevorzugt)** erfolgt in einer frühen Projektphase (vor dem UI-Neubau).

### Alternativen
- TFM-Upgrade erst nach UI-Migration

### Konsequenzen
- Frühzeitige Sichtbarkeit von Kompatibilitätsproblemen
- Vermeidet doppelte Migration (erst UI, dann TFM)

---

## ADR-0003 – Ziel-UI in Phase 1
- **Status:** Accepted
- **Datum:** 2026-09-25

### Kontext
Es stehen zwei UI-Ziele im Raum: WinUI 3 oder WPF (Fluent).

### Entscheidung
**Phase-1-Ziel: WPF mit modernem Fluent UI** für schnelle, risikoarme Funktionsparität.
**WinUI 3 wird explizit auf einen späteren Zeitpunkt verschoben** und erst nach stabiler WPF-Produktivphase erneut bewertet.

### Alternativen
- Sofortiger Wechsel auf WinUI 3 / Windows App SDK

### Konsequenzen
- Bessere Beherrschbarkeit von Tray-/Interop-Szenarien
- Schnellere Erreichung der Funktionsparität zur bestehenden WinForms-App
- WinUI 3 bleibt als optionaler Folgepfad möglich

---

## ADR-0004 – Hardware-Protokoll-Stabilität
- **Status:** Accepted
- **Datum:** 2026-09-25

### Kontext
Bestehende Geräte im Feld erwarten das bestehende Protokoll.

### Entscheidung
Das Protokoll (`syn`, `ack`, `get`, `NR=0..3`, Werte 0..100) bleibt kompatibel.

### Alternativen
- Neues Protokoll mit Breaking Changes

### Konsequenzen
- Abwärtskompatibilität bleibt erhalten
- Migration kann ohne Firmware-Zwang starten

---

## ADR-0005 – Zielversion für frühe .NET-Migration
- **Status:** Accepted
- **Datum:** 2026-09-25

### Kontext
Die Anwendung läuft aktuell auf `net7.0-windows`. Für die frühe Plattformmigration wurde eine konkrete Zielversion benötigt.

### Entscheidung
Die frühe Plattformmigration erfolgt auf **`net10.0-windows`**.

### Alternativen
- `net8.0-windows` als konservativerer Zwischenstand

### Konsequenzen
- Einheitliches Ziel für alle Folgeaufgaben (`01.02` ff.)
- Build-/Paketprüfung wird direkt gegen .NET 10 durchgeführt

---

## ADR-0006 – Funktionsschutz vor Visual Redesign
- **Status:** Accepted
- **Datum:** 2026-09-26

### Kontext
Die Modernisierung enthält starke UI/UX-Ziele, darf aber bestehende Kernfunktionen nicht gefährden.

### Entscheidung
Priorisierung wird verbindlich festgelegt: **Funktionalität → Stabilität → Architektur → UX → Visual Design → Cleanup**.

### Alternativen
- Design-first-Ansatz mit späterer Funktionsangleichung

### Konsequenzen
- Geringeres Regressionsrisiko
- Klarer Entscheidungsrahmen bei Zielkonflikten

---

## ADR-0007 – Keine stillen Funktionsänderungen
- **Status:** Accepted
- **Datum:** 2026-09-26

### Kontext
Bei Refactoring/Migration entstehen oft opportunistische „Nebenverbesserungen“, die Verhalten ändern können.

### Entscheidung
Funktionsänderungen ohne explizite Freigabe sind untersagt. Verbesserungen werden als **Potential Improvement** dokumentiert (Reason/Risk/Recommendation), aber nicht automatisch umgesetzt.

### Alternativen
- Direkte Umsetzung opportunistischer Verbesserungen während der Migration

### Konsequenzen
- Vorhersehbares Migrationsverhalten
- Höhere Nachvollziehbarkeit im Projektverlauf

---

## ADR-0008 – Verbindliche Phasen-Gates
- **Status:** Accepted
- **Datum:** 2026-09-26

### Kontext
Die Migration muss kontrolliert in kleinen Schritten erfolgen.

### Entscheidung
Nach jeder Phase sind verbindlich: Build, Fehlerbehebung, Warning-Prüfung, Regression gegen Abnahmecheckliste, Fortschrittsdokumentation.

### Alternativen
- Größere Umbaupakete mit später Sammelvalidierung

### Konsequenzen
- Frühes Erkennen von Problemen
- Kleinere, besser isolierte Änderungsblöcke

---

## ADR-0009 – Event-Driven Architektur bleibt verbindlich
- **Status:** Accepted
- **Datum:** 2026-09-26

### Kontext
Die Anwendung verarbeitet Hardware- und Audiozustände bereits eventbasiert. Polling-Ansätze würden unnötige Last und Komplexität einführen.

### Entscheidung
GameChatBalancer bleibt strikt event-driven. Für Arduino/Balancer/UI/Overlay werden keine kontinuierlichen Polling-Schleifen oder periodischen Timer eingeführt, sofern Ereignisse verfügbar sind.

### Erlaubte Ausnahmen
Timer nur für inhärent timerbasierte Fälle (Overlay-AutoHide, Animationstiming, Debouncing).

### Konsequenzen
- Niedrigerer Laufzeit-Overhead
- Klarere Zustandsflüsse über Events
- Bessere Skalierbarkeit für WPF/ViewModel-basierte Weiterentwicklung

---

## ADR-Template (für neue Entscheidungen)

```md
## ADR-XXXX – Titel
- Status: Proposed|Accepted|Superseded|Deprecated
- Datum: YYYY-MM-DD

### Kontext
...

### Entscheidung
...

### Alternativen
...

### Konsequenzen
...
