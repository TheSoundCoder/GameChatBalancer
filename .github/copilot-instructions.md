# Copilot Instructions

## Projektrichtlinien
- Der Nutzer möchte ein zentrales Umsetzungsbacklog und eine Designentscheidungsdokumentation als Steuerungs- und Gedächtnisartefakte im Repository; außerdem soll die Migration auf eine aktuelle .NET-Version (net10.0-windows) bereits in einer frühen Projektphase eingeplant werden.
- Der Nutzer möchte auf WPF mit Fluent UI migrieren und eine WinUI-3-Migration höchstens zu einem späteren Zeitpunkt evaluieren. Für die Modernisierung sollen Funktionalität und Stabilität strikt vor UX/Visuals priorisiert werden; Migration in kleinen Phasen mit Build/Warning/Test-Gates, WPF+Fluent als Ziel, WinUI 3 nur später optional.
- Der Nutzer bevorzugt Branch-Workflow: erst refactor/services-decoupling als Basisbranch, danach separater WPF-Upgrade-Branch darauf.