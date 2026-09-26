# Smoke-Test 01.04 – net10.0-windows

## Kontext
- Ziel: Kernfunktionen nach TFM-/Paketmigration verifizieren
- Build-Stand: erfolgreich auf `net10.0-windows`
- Pakete: `System.IO.Ports` = `10.0.12`, `System.Management` = `10.0.12`

## Automatisiert geprüft
- [x] Projekt-Build erfolgreich (`AudioControl.csproj`)
- [x] Keine Build-Fehler

## Manuell zu prüfen (Hardware-/UI-abhängig)
Referenz: `AI Agent/Abnahmecheckliste-Ist-Verhalten.md`

### A. Start- und Tray-Verhalten
- [ ] A1 Start im Tray
- [ ] A2 Tray-Menü/Status
- [ ] A3 Show/Hide/Exit

### B. COM-Port & Auto-Detect
- [ ] B1 COM-Portliste
- [ ] B2 Auto-Detect + Handshake (`syn/ack`)
- [ ] B3 Manuelle COM-Auswahl + Reconnect

### C. USB-Reconnect
- [ ] C1 USB trennen
- [ ] C2 USB wieder verbinden

### D. Hardware-Protokoll / Noise Reduction
- [ ] D1 Initialwertabruf (`get`)
- [ ] D2 NR setzen/Bestätigung (`NR=0..3`)

### E. Audio-Session-Erkennung & Assignment
- [ ] E1 Prozessliste + Refresh
- [ ] E2 Drag & Drop Assignment

### F. Mix-Logik
- [ ] F1 <50 / =50 / >50 Verhalten
- [ ] F2 Invert-Verhalten

### G. Persistenz
- [ ] G1 Speichern
- [ ] G2 Wiederherstellung nach Neustart

### H. Debug (optional)
- [ ] H1 Debug-Ausgaben

## Ergebnisstatus
- **Automatisierter Teil:** bestanden
- **Gesamtstatus 01.04:** offen bis manuelle Validierung abgeschlossen ist

## Hinweis
Für Abschluss von 01.04 werden manuelle Tests am Zielsystem mit angeschlossener Hardware benötigt.
