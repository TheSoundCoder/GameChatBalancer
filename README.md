# GameChatBalancer
GameChatBalancer is an **opensource hardware audio balancer** for Windows PCs. The functionality is inspired by the Chatmix / Gamevoice mix function of some available headsets.

GameChatBalancer lets you use an Arduino (or compatible) based knob or slider to **balance the volume of chat-software and games** so that you can **easily
tune the audio volume while playing** without any interruption by just turning a knob or moving a slider.

The project consists of two components. A small **systray application** for your PC and an Arduino based **cheap to build** hardware.

# Table of contents
- [Features](https://github.com/TheSoundCoder/GameChatBalancer#Features)
- [Build Hardware](https://github.com/TheSoundCoder/GameChatBalancer#build-hardware)
  - [Schematic](https://github.com/TheSoundCoder/GameChatBalancer#schematic)
  - [Material](https://github.com/TheSoundCoder/GameChatBalancer#material)
  - [Housing](https://github.com/TheSoundCoder/GameChatBalancer#housing)
- [Make your own GameChatBalancer](https://github.com/TheSoundCoder/GameChatBalancer#make-your-own-gamechatbalancer)
- [Licences](https://github.com/TheSoundCoder/GameChatBalancer#licences)
- [Roadmap](https://github.com/TheSoundCoder/GameChatBalancer#roadmap)
- [Special Thanks](https://github.com/TheSoundCoder/GameChatBalancer#special-thanks)
- [Help & Feedback](https://github.com/TheSoundCoder/GameChatBalancer/#help--feedback-wanted)

# Features
1. Hardware
   - Only sends data via the serial port if the chose volume level changed -> **no permanent datastream**
   - **Noise reduction** in case you are using cheap components - especially the potentiometer
     - Levels: Off, Low, Medium, High (default)
     - Adjustable at runtime via Systray Application

2. Systray Application
   - Easy configuration via GUI - Assign applications via **drag and drop**
   - Assign **multiple applications** to the "Game" and "Chat" category
   - **Auto-detect** the Arduino based hardware
   - The application detects when the hardware is removed and will automatically reconnect when plugged in again (**USB event based**)
   - User settings are stored automatically and restored during application startup
   - Invert the values received by the hardware in case your wiring is mixed up


See the application in action:

https://user-images.githubusercontent.com/130736237/235218694-252510ab-6273-4d05-bc87-e7d56919cefa.mp4


This is how it look like in the Windows Audio mixer:

https://user-images.githubusercontent.com/130736237/235218894-d49eed7c-69e4-4d56-9448-dae65230d700.mp4


# Build Hardware
Everything you need to build your own GameChatBalancer ist listed in the [Pricing indication](https://github.com/TheSoundCoder/AudioControl#pricing-indication).
- The potentiometer is connected to an analog pin of your Arduino (or compatible) board. It is powered from the board's 5V (or 3.3V for a Seeed board) output (see schematic)
  - GND = black
  - 5V / 3.3V = red
  - Signal = green
- The board is conected via an USB cable to the PC. **Make sure to check what kind of USB cable is needed for your board.**
- As listed in the Pricing indication I used a **Seeed Xiao SAMD21 Cortex M0+** instead of an Arduino. It comes cheap (5,60€), has a lot of analog connectors and a very small footprint (20mm x 17,5mm x 3,5mm) so that it fits into a small housing. Last but not least it has a USB-C connector.
 
## Schematic
In the schematic below, you can find two options:
1. *Left* is the setup with the *Seeed Xiao SAMD21 Cortex M0+*. **Ensure to connect the potentiometer to the 3.3V output** of the board as the 5V will probably kill the board's microcontroller.
2. *Right* is the setup with an *Arduino*.

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer_schematic_2.png)

## Material
**Please keep in mind that a cheap potentiometer can cause noise**, so that noise reduction needs to be switched on in the Systray application.
I use a ~ 3€ poti with a **low tolerance of +-5%** in my device and noise reduction switched off. Cheap potentiometers usually have a tolerance of +-20%.

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
You can use anything as a housing for your GameChatBalancer - even a matchbox if you do not have a 3D-Printer.

I decided to design and 3D-print one by myself. If you like the "design" you can download either the STL files or GCODE files [here](https://github.com/TheSoundCoder/AudioControl/tree/master/assets/3d-model).

<img src="https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer3.jpg" height="300"> <img src="https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer2.jpg" height="300" style="fillColor: 'ff0000'">

# Make your own GameChatBalancer

If you want to build your own GameChatBalancer or get details on how to handle the systray application. Please have a look at the dedicated [manual](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md).


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
Depending on my free time I would like to make the complete setup flexible so that additional potentiometers are supported. Those could be used to (e.g.):
- Control the master volume of your PC
- Assign single or multiple applications to a potentiometer

I am currently thinking of how the Systray UI and configuration of the Arduino can be done flexible so that no new sketch needs to be uploaded if the overall configuration changes.

# Special Thanks
I would like to thank **@sverrirs for his Gist AudioManager.cs** which accelerated the development and for sure saved me from some pain during the development. I used and modified AudioManager.cs to my needs so that GameChatBalancer is able to adjust the volume of more than one application by application name in one single call.

# Help & Feedback wanted
I created a first issue where help would be very welcome. Maybe it is not an easy one, but if somebody has a good idea or would like to contribute to it, this would be very welcome. This is my first (public) repo, so please be patient with me.

Feedback is also more than welcome. Please let me know if you (dis-) LIKE something or if you see issues in the code have good hints how to improve.
