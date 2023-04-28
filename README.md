# GameChatBalancer
GameChatBalancer is an **opensource hardware audio balancer** for Windows PCs. The functionality is inspired by the Chatmix function of some available headsets.

GameChatBalancer lets you use an Arduino (or similar boards) based knob or slider to **balance the volume of chat-software and games** so that you can **easily
tune the audio volume while playing** without any interruption by just turning a knob or moving a single slider.

The project consists of two components. A small **systray application** for your PC and an Arduino based **cheap to build** hardware.

# Table of contents
- [Features](https://github.com/TheSoundCoder/AudioControl#Features)
- [Build Hardware](https://github.com/TheSoundCoder/AudioControl#build-hardware)
  - [Schematic](https://github.com/TheSoundCoder/AudioControl#schematic)
  - [Material](https://github.com/TheSoundCoder/AudioControl#material)
  - [Housing](https://github.com/TheSoundCoder/AudioControl#housing)
- [Installation](https://github.com/TheSoundCoder/AudioControl#installation)
- [Licences](https://github.com/TheSoundCoder/AudioControl#licences)

- [Special Thanks](https://github.com/TheSoundCoder/AudioControl#special-thanks)

# Features
1. Hardware
   - Only sends data via the serial port if something changed - no permanent datastream
   - **Noise reduction** in case you are using cheap components - especially the potentiometer
     - Levels: Off, Low, Medium, High (default)
     - Adjustable at runtime via Systray Application

2. Systray Application
   - Easy configuration via GUI - Assign applications via **drag and drop**
   - Assign **multiple applications** to the "Game" and "Chat" category
   - **Auto-detect** the Arduino based hardware
   - The application recognises when the hardware is removed and will automatically reconnect when plugged in again (**USB event based**)
   - User settings are stored automatically and restored during application startup.
   - Invert the values received by the hardware in case your wiring is mixed up.

# Build Hardware
Everything you need to build your own GameChatBalancer ist listed in the [Pricing indication](https://github.com/TheSoundCoder/AudioControl#pricing-indication).
- The potentiometer is connected to an analog pin of your Arduino (or compatible) board. It is powered from the board's 5V (or 3.3V for a Seeed board) output (see schematic)
  - GND = black
  - 5V / 3.3V = red
  - Signal = green
- The board is conected via an USB cable to the PC. Make sure to check what kind of USB cable is needed for your board.
- As listed in the Pricing indication I used a **Seeed Xiao SAMD21 Cortex M0+** instead of an Arduino. It comes cheap (5,60€), has a lot of analogue connectors and a very small footprint (20mm x 17,5mm x 3,5mm) to fit into a small housing. Last but not least it has a USB-C connector.
 
## Schematic
In the schematic below, you find two options:
1. Left is the setup with the Seeed Xiao SAMD21 Cortex M0+. **Ensure to connect the potentiometer to the 3.3V output** of the board as the 5V will probably kill the board's microcontroller.
2. Right is the setup with an Arduino.

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer_schematic_2.png)

## Material
**Please keep in mind that a very cheap potentiometer causes noise** so that noise reduction needs to be switched on. It absolutely makes sense to spend 2-3€ instead of 0,45€ as listed below. I use a ~ 3€ poti with a **tolerance of +-5%** in my device and noise reduction switched off. Cheap potentiometers can have a tolerance of +-20%.

**Important**: make sure to buy a **linear** potentiometer, not a logarithmic one!

Item | Price (€)|Link
-----|------|-------
Microcontroller: Seeed Xiao SAMD21 Cortex M0+|5,60|https://www.berrybase.de/en/seeeduino-xiao-arduino-microcontroller-samd21-cortex-m0
Potentiometer Noname 10k (tolerance: +-20%)|0,45|https://www.berrybase.de/en/drehpotentiometer-10k-937-liegend-mono-6mm-loetoesen?c=164
*Optional:* Potentiometer Alpha RV16AF20KB10KM (tolerance: +-5%)|2,99|https://www.conrad.de/de/p/alpha-rv16af20kb10km-dreh-potentiometer-mono-200-mw-10-k-1-st-1694304.html#productDownloads 
Knob|0,65|https://www.berrybase.de/en/drehknopf-fuer-geraendelte-6-0mm-achse-mit-anzeige-16x16mm-schwarz/grau?c=164
Copper wire isolated|1,50|https://www.berrybase.de/en/kupferschaltdraht-isoliert-oe0-5mm-10m
**Total**|**8,20 - 10,74**|

If you use the listed microcontroller you will additionally need a **USB-C cable**.

## Housing
You can use anything as a housing for your controller - even a matchbox if you do not have a 3D-Printer.

I decided to design and 3D-print one by myself. If you like the "design" you can download either the STL files or GCODE files [here](https://github.com/TheSoundCoder/AudioControl/tree/master/assets/3d-model).

<img src="https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer3.jpg" height="300"> <img src="https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer2.jpg" height="300" style="fillColor: 'ff0000'">

# Installation

## Arduino
**Prerequisite**: You should have the **Arduino IDE** installed on your PC. You can download it [here](https://www.arduino.cc/en/software).
- Download the file **GCBalancer.ino** from the latest release of this project (you should find it at the right side under "Releases").
- Open the downloaded sketch AudioControl.ino with the installed Arduino IDE
- Follow the instructions "How to upload a sketch with the Arduino IDE 2" from the [Arduino website] (https://docs.arduino.cc/software/ide-v2/tutorials/getting-started/ide-v2-uploading-a-sketch)

### Test
After uploading the sketch, open the serial monitor of thr Arduino IDE by choosing *Tools* ->  *Serial Monitor* (or by pressing `strg` + `shift` + `M`).

If you turn the potentiometer (or move the slider) you should see **values between 0 and 100** coming in. If this is true: Congratulations! Your device is working as it should.

**Close the serial monitor.** Keeping it open might prevent you from uploading a sketch in the future.

## Systray application
- Download the file **GCBalancer_win.zip** from the latest release of this project (you should find it at the right side under "Releases").
- Store the content of the ZIP file in any folder of your choice.
- Run **GCBalancer.exe** from the folder above.
- **Optional**: Add GCBalancer.exe to **Autostart** if you want to run it at system startup.
  - Press `Win` + `R` to open the Run menu
  - Type shell:startup and press enter
  - Right click and create a new link to GCBalancer.exe

# Manual
The application will start in the systray:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GCB_Systray_Expanded.png)

Clicking on **Show** will open the GUI so that you can configure for which applications you want to modify the volume:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GCB_settings.png)

# Pictures / Video
![](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/assets/GCBalancer.mp4)
<img src="https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GCB_settings.png" usemap="#mymap">
<map name="mymap"><area shape="rect" coords="34,44,270,350" style="fillColor: 'ff0000'></map>

# Licences
Icons used under [Creativecommons Attribution 4.0](https://creativecommons.org/licenses/by/4.0/legalcode) from https://icon-icons.com:
Designer | Iconpack | Icon Name | Image
---------|----------|---------|---------
Perpixel | Mobile Basic Vol. 1 | conversation Icon | <img src="https://github.com/TheSoundCoder/AudioControl/blob/master/AudioControl/Resources/ico_chat3.png" width="48">
Iconicons | Icons - line | Game, controller icon | <img src="https://cdn.icon-icons.com/icons2/3831/PNG/512/game_controller_icon_234136.png" width="48">
CoreUI | CoreUI Free | Equalizer Icon | <img src="https://cdn.icon-icons.com/icons2/2385/PNG/512/equalizer_icon_144361.png" width="48">
Steve Shoger | Zondicons | Bug Icon | <img src="https://cdn.icon-icons.com/icons2/2065/PNG/512/bug_icon_125029.png" width="48">
Yannick | General Icons | Refresh Icon | <img src="https://cdn.icon-icons.com/icons2/562/PNG/512/refresh-page-arrow-button_icon-icons.com_53909.png" width="48">

# Roadmap
Depending on my free time I would like to make the complete setup flexible so that additional potentiometers are supported. Those can then be used to (e.g.):
- Control the master volume of your PC
- Assign single or multiple applications to a potentiometer

From controlling the audio volume this is very easy to implement. I am currently thinking of how the Systray UI and configuration of the Arduino can be done flexible so that no new sketch needs to be uploaded.

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
    - https://social.msdn.microsoft.com/Forums/es-ES/65a2064f-2d6a-4ecc-8076-60c72cb7070d/wpf-c-save-controls-created-at-runtime?forum=wpf
