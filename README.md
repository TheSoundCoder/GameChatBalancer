# GameChatBalancer

GameChatBalancer is an **opensource hardware audio balancer** for Windows PCs. The functionality is inspired by the Chatmix function of some available headsets.

GameChatBalancer lets you use an Arduino (or similar boards) based knob or slider to **balance the volume of chat-software and games** so that you can **easily
tune the audio volume while playing** without any interruption by just turning a knob or moving a single slider.

The project consists of two components. A small **systray application** for your PC and an Arduino based **cheap to build** hardware.

# Features
1. Hardware
   - Only sends data via the serial port if something changed - no permanent datastream
   - Noise reduction in case you are using cheap components - especially the potentiometer
     - Levels: Off, Low, Medium, High (default)
     - Adjustable via Systray Application

2. Systray Application
   - Easy configuration via GUI - no extra configuration file editing needed
   - Auto-detect the Arduino based hardware
   - The application recognises when the hardware is removed and will automatically reconnect when plugged in again (USB event based)
   - Adjust Noise reduction level of the Hardware during runtime
   - User settings are stored automatically and restored during application startup.

# Build Hardware
![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer_schematic.png)

# Installation

# Licences
Icons from https://icon-icons.com:
Designer | Iconpack | Icon
---------|----------|---------
Perpixel | Mobile Basic Vol. 1 | conversation Icon

# Pricing indication
**Please keep in mind that a very cheap potentiometer causes noise** so that noise reduction needs to be switched on. It absolutely makes sense to spend 2-3€ instead of 0,45€ as listed below. I use a ~ 3€ poti with a **tolerance of +-5%** in my device and noise reduction switched off. Cheap potentiometers can have a tolerance of +-20%.
Item | Price (€)|Link
-----|------|-------
Microcontroller: Seeed Xiao SAMD21 Cortex M0+|5,60|https://www.berrybase.de/en/seeeduino-xiao-arduino-microcontroller-samd21-cortex-m0
Potentiometer Noname 10k (tolerance: +-20%)|0,45|https://www.berrybase.de/en/drehpotentiometer-10k-937-liegend-mono-6mm-loetoesen?c=164
*Optional:* Potentiometer Alpha RV16AF20KB10KM (tolerance: +-5%)|2,99|https://www.conrad.de/de/p/alpha-rv16af20kb10km-dreh-potentiometer-mono-200-mw-10-k-1-st-1694304.html#productDownloads 
Knob|0,65|https://www.berrybase.de/en/drehknopf-fuer-geraendelte-6-0mm-achse-mit-anzeige-16x16mm-schwarz/grau?c=164
Copper wire isolated|1,50|https://www.berrybase.de/en/kupferschaltdraht-isoliert-oe0-5mm-10m
**Total**|**8,20 - 10,74**|

If you use the listed microcontroller you will additionally need a **USB-C cable**.

# Special Thanks
First of all I would like to thank **sverrirs for his Gist AudioManager.cs** which accelerated the development and for sure saved me from some pain during the development. I used and modified AudioManager.cs to my needs so that GameChatBalancer is able to adjust the volume of more than one application by application name in one single call.

# Issues
- [x] Implement options for noise reduction (Off(0) - Low(1) - Medium(2) - High(3))
  - [x] Default: High
  - [x] Customizable via C# application
  - [x] Set / Get value during startup of the application
  - [ ] [Optional] Auto-Adjust mode for noise reduction

## Optional
- [ ] Screen overlay to see volume adjustments Ingame
https://github.com/michel-pi/GameOverlay.Net
- [ ] Migrate to modern UI
