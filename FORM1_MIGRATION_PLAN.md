# FORM1 Migration Plan

## Scope (Phase 1 only)
This document captures the current Form1 dependency graph and a safe migration path.  
No Form1 deletion is performed in this phase.

---

## 1) Current responsibilities of `Form1`

### Application host / lifecycle
- App startup host (`Program.Main` starts `new Form1()`)
- Subscribes global lifecycle events:
  - `Application.ApplicationExit`
  - `SystemEvents.SessionEnding`
- Owns startup sequencing (`Form1_Load`), including initialization order.
- Owns shutdown sequencing (`OnApplicationExit`, `PersistSettingsImmediate`, service stop/shutdown).

### Tray behavior
- Owns `NotifyIcon` and interaction entry points (`trayicon_MouseUp`).
- Creates/displays `SystrayPopupWindow` and positions it.
- Handles tray commands (`Show`/`Exit`) via `openToolStripMenuItem_Click` and `closeToolStripMenuItem_Click`.

### WPF window hosting/orchestration
- Creates `MainWindow` with many callbacks/providers.
- Keeps `MainWindow` and systray popup synchronized with runtime state.
- Intercepts WPF main-window close (`WpfMainWindow_Closing`) to hide instead of exit.

### Audio control orchestration
- Calls `IAudioBalanceService.ApplyBalance(...)`.
- Maintains current balance mirror in WinForms controls (`trackBar1`, labels) and propagates to WPF UI.
- Responds to incoming Arduino volume values via `controlVolume(...)`.

### Arduino/serial orchestration
- Uses `ISerialDeviceService` and reacts to `ConnectionStatusChanged`.
- Triggers startup handshake/value fetch (`GetVol`) and noise reduction command send.
- Applies noise reduction/invert settings and propagates to UI surfaces.

### Process/audio-session detection orchestration
- Uses `IAudioSessionService`.
- Subscribes to `AudioSessionsChanged` and refreshes assignment lists/snapshots.

### Settings and assignment persistence orchestration
- Reads/writes `ISettingsStore`.
- Maintains `GAME`/`CHAT` assignment strings and app path mapping.
- Persists state at runtime and on shutdown.

### Hotkeys (software control when Arduino disconnected)
- Registers/unregisters Win32 hotkeys in host window (`WndProc`).
- Applies step/snap/center logic and routes result through balance apply path.

### Debug/log sink and UI debug state
- Maintains in-memory debug log list.
- Routes logs into legacy WinForms textbox and WPF debug page callbacks.

### Legacy WinForms UI state bridge (technical debt)
- Exposes/uses WinForms control state as runtime state source:
  - COM dropdown
  - invert checkbox
  - volume labels/trackbar
  - listboxes for assignment
- This state is currently used by serial/audio paths indirectly.

---

## 2) All known consumers/dependencies on `Form1`

## Direct references
- `AudioControl/Program.cs`
  - `WinFormsApplication.Run(new Form1())`
- `AudioControl/SerialDeviceService.cs`
  - Constructor signature `SerialDeviceService(Form1 form)`
  - Calls `USBandCOM.HandOverForm(form)`
- `AudioControl/AudioSessionService.cs`
  - Constructor signature `AudioSessionService(Form1 form)`
  - Calls `AudioManager.AudioManager.HandOverForm(form)`
- `AudioControl/USBandCOM.cs`
  - Static field `MainForm` of type `AudioControl.Form1`
  - Extensive calls to `MainForm.*` for logging/UI-thread marshaling/state updates
- `AudioControl/AudioManager.cs`
  - Static field `MainForm` of type `AudioControl.Form1`
  - Uses `MainForm.Debug` + `MainForm.SendToLog(...)`

## Indirect dependencies
- WPF UIs depend on Form1-owned callback graph:
  - `GameChatBalancer.Wpf/MainWindow.xaml.cs` (callbacks provided by Form1)
  - `GameChatBalancer.Wpf/SystrayPopupWindow.xaml.cs` (callbacks provided by Form1)
- USB watcher service (`UsbWatcherService`) and serial layer depend on `USBandCOM`, which currently depends on Form1.

## Startup/lifecycle dependency
- Entire application runtime currently requires Form1 as process host window/message pump anchor.

---

## 3) Responsibility -> proposed destination

| Responsibility in Form1 | Proposed destination |
|---|---|
| App startup orchestration | New `AppHost` / `ApplicationLifecycleService` (non-UI) |
| Tray icon + menu/popup wiring | New `ITrayHostService` (WinForms `NotifyIcon` can remain internal implementation detail) |
| MainWindow creation/show/hide policy | New `IWindowCoordinator` / `IUiCoordinator` |
| Runtime app state (connected, balance, noise, invert, assignments) | New `IAppStateStore` + event stream |
| Software-control hotkeys | New `ISoftwareControlHotkeyService` (host-level, independent of MainWindow) |
| Serial value intake + command dispatch coordination | New `IArduinoController` (uses `ISerialDeviceService`) |
| Balance application orchestration | Keep `IAudioBalanceService`, add thin `IBalanceController` for orchestration/state events |
| Audio-session refresh/reassignment triggering | New `IAssignmentRefreshService` (uses `IAudioSessionService`) |
| Debug logging fan-out | New `IAppLogger` / `IDebugLogStore` abstraction |
| Settings persistence flow | Keep `ISettingsStore`, move shutdown save into lifecycle service |

Guiding rule: no `Form1` reference in core/services; UI consumes abstractions/events.

---

## 4) Required interfaces/services (incremental)

## Reuse existing
- `ISerialDeviceService`
- `IAudioSessionService`
- `IAudioBalanceService`
- `IUsbWatcherService`
- `ISettingsStore`
- `IAutoStartService`

## Introduce (small focused components)
- `IAppStateStore` (current state + change events)
- `IArduinoController` (serial connect/disconnect, incoming value event, NR ack event, send commands)
- `IBalanceController` (apply balance, emit balance changed)
- `ISoftwareControlHotkeyService` (register/unregister, hotkey events)
- `ITrayHostService` (notify icon lifecycle, show/hide popup commands)
- `IUiCoordinator` (open main window, sync UI state, close policy)
- `IAppLifecycleService` (startup/shutdown sequencing and disposal)
- `IDebugLogStore` (log sink + observable log entries)

No “Form1Service” aggregation class.

---

## 5) Required event flows (event-driven, no polling)

## Serial/Arduino flow
1. `IArduinoController.ConnectionStatusChanged`
2. `IAppStateStore` updates connected/port state
3. Subscribers update:
   - Tray popup state
   - MainWindow state
   - Hotkey service registration policy

## Arduino value flow
1. `IArduinoController.ValueReceived(float)`
2. `IBalanceController.ApplyInput(value, invert)`
3. `IBalanceController.BalanceChanged` (display/game/chat)
4. Subscribers:
   - Audio controller/session volume application
   - MainWindow balance UI
   - Systray popup balance UI

## Noise reduction flow
1. UI command -> `IArduinoController.SetNoiseReduction(level)`
2. state enters applying
3. NR confirmation event from serial -> state confirmed
4. timeout/error event -> state error

## Audio session change flow
1. `IAudioSessionService.AudioSessionsChanged`
2. assignment refresh service recomputes available apps
3. `IAppStateStore.AvailableAppsChanged`
4. MainWindow refreshes assignment lists

## Lifecycle flow
- Startup: initialize services in host service order (settings -> serial/usb watcher -> session monitor -> tray)
- Shutdown: reverse order with deterministic dispose/unsubscribe.

---

## 6) Event handlers currently wired through Form1

## Framework/lifecycle
- `Application.ApplicationExit += OnApplicationExit`
- `SystemEvents.SessionEnding += OnSessionEnding`

## Service events
- `serialDeviceService.ConnectionStatusChanged += SerialDeviceService_ConnectionStatusChanged`
- `audioSessionService.AudioSessionsChanged += AudioSessionService_AudioSessionsChanged`

## WinForms UI handlers (Designer)
- Tray: `trayicon_MouseUp`
- Context menu / commands: `openToolStripMenuItem_Click`, `closeToolStripMenuItem_Click`, `Settings_Opening`
- Controls: `ddl_ComPort_SelectedIndexChanged`, `ddlNoiseReduction_SelectedIndexChanged`, `cb_Debug_CheckedChanged`, `cb_invert_CheckedChanged`, drag/drop handlers, etc.

## Win32 message hook
- `WndProc` for software hotkeys.

---

## 7) Static/global Form1 access identified

- `USBandCOM.MainForm` static field (`AudioControl.Form1`)
- `USBandCOM.HandOverForm(Form1 f)`
- `AudioManager.MainForm` static field (`AudioControl.Form1`)
- `AudioManager.HandOverForm(Form1 f)`

These are the primary blockers for Form1 deletion.

---

## 8) WinForms-specific access caused by Form1 coupling

Core/runtime paths currently read/write WinForms controls indirectly via Form1:
- COM state via `SystrayCom`, `Connected`, `Fill_ddl_ComPort`, `Initialized`
- Balance state via `trackBar1`, `lbl_game_vol`, `lbl_chat_vol`, `lbl_absoluteval`
- Invert state via `cb_invert`
- Debug output via `textBox1`
- Assignment management through listbox manipulation methods

This must move to neutral state/services before Form1 removal.

---

## 9) Initialization/shutdown responsibilities currently in Form1

## Constructor
- Creates/default-injects service instances
- Wires serial connection event
- syncs assignment-path store from persisted settings

## Load
- Initializes assignment/process lists
- Normalizes defaults (`NoiseReduction`, `ComPort`)
- Starts USB watcher
- Opens serial port and requests initial value
- Registers startup hotkeys according to connection state
- Hides legacy form and shows tray
- Initializes systray popup state
- Starts audio session monitoring

## Runtime close/hide
- MainWindow closing intercepted (hide-to-tray behavior)

## Exit/session ending
- Unregister hotkeys
- Stop session monitoring and unsubscribe
- Persist settings
- Stop USB watcher
- Shutdown serial service
- Unsubscribe `SystemEvents.SessionEnding`

---

## 10) What would break if Form1 were removed now

- Application startup (entry point instantiates Form1)
- Serial/USB pipeline (`SerialDeviceService` + `USBandCOM`) due to hard `Form1` dependency
- Audio session service logging bridge (`AudioSessionService` -> `AudioManager.HandOverForm`)
- Tray icon and popup lifecycle (currently hosted by Form1)
- Software-control hotkeys (currently in Form1 `WndProc`)
- Current state synchronization between serial/audio/tray/WPF callbacks
- Shutdown/disposal sequencing currently centralized in Form1

---

## Migration order (recommended, incremental)

1. Introduce `IDebugLogStore` and remove `AudioManager`/`USBandCOM` direct `Form1` logging dependency.
2. Refactor `USBandCOM` to depend on neutral callbacks/events/state interface (no `Form1` type).
3. Refactor `SerialDeviceService` constructor to remove `Form1` parameter.
4. Refactor `AudioSessionService` constructor to remove `Form1` parameter.
5. Introduce `IAppStateStore` for connected/port/balance/noise/invert/assignments runtime state.
6. Extract software hotkeys into `ISoftwareControlHotkeyService` (host-level, not MainWindow-bound).
7. Extract tray host/popup control into `ITrayHostService`.
8. Extract WPF window coordination (`IUiCoordinator`) from Form1.
9. Move startup/shutdown orchestration into `IAppLifecycleService`.
10. Update `Program.Main` to start new host service (not Form1).
11. Search and remove remaining `Form1` references.
12. Delete `Form1.cs`, `Form1.Designer.cs`, `Form1.resx` and obsolete wiring.
13. Final build/tests + runtime validation checklist.

---

## Potential risks

- Hidden assumptions on WinForms control values as source-of-truth.
- Event-threading regressions during serial callbacks and UI dispatch transitions.
- Hotkey registration conflicts if ownership changes incorrectly.
- Resource leaks (serial port, watcher, session monitor, event subscriptions) if lifecycle extraction order is wrong.
- Tray behavior regressions if host window/message loop responsibilities are moved without equivalent handling.

---

## Final deletion checklist

- [ ] `Program.Main` no longer creates `new Form1()`.
- [ ] `SerialDeviceService` has no `Form1` constructor dependency.
- [ ] `AudioSessionService` has no `Form1` constructor dependency.
- [ ] `USBandCOM` contains no `Form1` type/member usage.
- [ ] `AudioManager` contains no `Form1` type/member usage.
- [ ] No `Form1`/`form1` references in solution-wide search (except migration docs/changelog).
- [ ] Tray popup and main window state sync driven by services/events, not Form1 callbacks.
- [ ] Startup without opening MainWindow still initializes core/tray/hotkeys correctly.
- [ ] Shutdown disposes serial/USB/audio/hotkeys cleanly and unsubscribes events.
- [ ] Remove `Form1.cs`, `Form1.Designer.cs`, `Form1.resx` from project.
- [ ] Full solution build passes.
- [ ] Existing tests pass.
- [ ] Manual smoke checks pass: startup, tray, reconnect, balance, hotkeys, settings persistence, exit/restart.

---

## Progress update (Phase 2 - in progress)

Completed refactorings:
- Introduced `IDiagnosticsSink` (generic diagnostics/log contract).
- Introduced `ISerialHostBridge` (small host callback/dispatch contract for serial layer).
- `AudioManager` no longer stores a static `Form1` reference for diagnostics:
  - replaced with `IDiagnosticsSink` via `HandOverDiagnosticsSink(...)`.
- `AudioSessionService` constructor migrated from `Form1` to `IDiagnosticsSink`.
- `USBandCOM` migrated from static `Form1 MainForm` to `ISerialHostBridge`:
  - logging, dispatching, connected state, COM refresh, NR confirm/send, and volume callback now go through bridge methods.
- `SerialDeviceService` constructor migrated from `Form1` to `ISerialHostBridge`.
- `Form1` now implements `ISerialHostBridge` (and therefore `IDiagnosticsSink`) as an adapter during transition.

Resulting dependency improvements:
- Removed direct `Form1` type dependency from:
  - `AudioControl/AudioManager.cs`
  - `AudioControl/AudioSessionService.cs`
  - `AudioControl/USBandCOM.cs`
  - `AudioControl/SerialDeviceService.cs`

Validation:
- Solution build successful after migration.
