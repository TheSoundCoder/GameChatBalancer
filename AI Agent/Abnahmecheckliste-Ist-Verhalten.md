# GameChatBalancer – Abnahmecheckliste (Ist-Verhalten / Regression)

> Zweck: Referenz für alle folgenden Migrationen (.NET-Upgrade, Entkopplung, WPF).
> 
> Ergebnis gilt als „bestanden“, wenn das beobachtete Verhalten der aktuellen WinForms-Version entspricht.

## Testumgebung
- Windows 11
- Aktuelle WinForms-Build der App
- Angeschlossenes Arduino/kompatibles Gerät mit bestehendem Protokoll (`syn/ack/get/NR`)
- Mindestens eine Chat-App (z. B. Discord) und eine Game-/Audio-App aktiv

## A. Start- und Tray-Verhalten

### A1 – Start im Tray
- [ ] App starten
- [ ] Prüfen: Fenster startet minimiert/versteckt
- [ ] Prüfen: Tray-Icon sichtbar

**Bestanden, wenn:** App primär im Tray lebt und nicht als normales Vordergrundfenster startet.

### A2 – Tray-Menü/Status
- [ ] Rechtsklick auf Tray-Icon
- [ ] Prüfen: COM-Statuszeile sichtbar
- [ ] Prüfen: letzte empfangene Volume-Zahl sichtbar
- [ ] Prüfen: Menüeinträge „Show“ und „Exit“ vorhanden

**Bestanden, wenn:** alle Status-/Bedienelemente im Kontextmenü vorhanden sind.

### A3 – Show/Hide/Exit
- [ ] „Show“ öffnet Settings-Fenster
- [ ] Fenster minimieren ⇒ Fenster versteckt sich wieder, Tray bleibt aktiv
- [ ] „Exit“ beendet App vollständig

**Bestanden, wenn:** Lifecycle exakt wie bisher funktioniert.

---

## B. COM-Port & Auto-Detect

### B1 – COM-Portliste
- [ ] Settings öffnen
- [ ] COM-Dropdown enthält „Auto“
- [ ] Verfügbare COM-Ports werden gelistet

### B2 – Auto-Detect + Handshake
- [ ] COM auf „Auto“ setzen
- [ ] App (neu) verbinden lassen
- [ ] Prüfen: Verbindung wird aufgebaut (Connected-Status)

**Bestanden, wenn:** Gerät über Handshake (`syn/ack`) erkannt und verbunden wird.

### B3 – Manuelle COM-Auswahl
- [ ] Konkreten COM-Port wählen
- [ ] Prüfen: Reconnect erfolgt sauber
- [ ] Prüfen: Status im UI/Tray aktualisiert

**Bestanden, wenn:** Portwechsel ohne Neustart stabil funktioniert.

---

## C. USB-Reconnect

### C1 – USB trennen
- [ ] Gerät im laufenden Betrieb abziehen
- [ ] Prüfen: Verbindung als getrennt markiert

### C2 – USB wieder verbinden
- [ ] Gerät wieder einstecken
- [ ] Prüfen: automatische Wiederverbindung ohne App-Neustart

**Bestanden, wenn:** eventbasierte Erkennung + Reconnect funktionieren.

---

## D. Hardware-Protokoll / Noise Reduction

### D1 – Initialwert abrufen
- [ ] Nach erfolgreicher Verbindung prüfen, ob Wertabruf erfolgt (`get`-Flow)
- [ ] UI zeigt plausiblen Wert (0..100)

### D2 – Noise Reduction setzen
- [ ] NR-Level ändern (Off/Low/Medium/High)
- [ ] Prüfen: Hardware bestätigt NR (Checkbox/Bestätigung)

**Bestanden, wenn:** NR-Kommandos zuverlässig gesendet und bestätigt werden.

---

## E. Audio-Session-Erkennung & Assignment

### E1 – Prozessliste aktualisieren
- [ ] Audio-App starten
- [ ] Refresh in App auslösen
- [ ] Prüfen: Prozess erscheint in „Available programs“

### E2 – Drag&Drop-Zuordnung
- [ ] Prozess nach CHAT ziehen
- [ ] Prozess nach GAME ziehen
- [ ] Prüfen: Einträge erscheinen korrekt in den Ziel-Listen

**Bestanden, wenn:** Zuordnung zuverlässig per Drag&Drop klappt.

---

## F. Mix-Logik (Kernfunktion)

### F1 – Grenzwerte
- [ ] Eingehender Wert < 50 ⇒ GAME wird abgesenkt, CHAT bleibt 100
- [ ] Eingehender Wert = 50 ⇒ beide 100
- [ ] Eingehender Wert > 50 ⇒ CHAT wird abgesenkt, GAME bleibt 100

### F2 – Invert
- [ ] „Invert“ aktivieren
- [ ] Prüfen: Richtung der Regelung ist logisch invertiert

**Bestanden, wenn:** Mix-Verhalten der bisherigen Logik entspricht.

---

## G. Persistenz

### G1 – Einstellungen speichern
- [ ] GAME/CHAT-Zuordnung ändern
- [ ] COM-Port/NR/Invert ändern
- [ ] App beenden und neu starten

### G2 – Wiederherstellung
- [ ] Prüfen: alle geänderten Werte sind nach Neustart vorhanden

**Bestanden, wenn:** User-Settings konsistent gespeichert und geladen werden.

---

## H. Debug/Diagnose (optional, aber empfohlen)

### H1 – Debug aktivieren
- [ ] Debug-Checkbox setzen
- [ ] Prüfen: erwartete Logeinträge erscheinen

**Bestanden, wenn:** Diagnoseinformationen reproduzierbar sichtbar sind.

---

## Abnahmeprotokoll

| Datum | Build/Commit | Ergebnis | Bemerkungen |
|---|---|---|---|
| YYYY-MM-DD |  |  |  |
