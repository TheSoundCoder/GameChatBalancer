# GameChatBalancer

GameChatBalancer is an **opensource hardware audio balancer** for Windows PCs. The functionality is inspired by the Chatmix function of some available headsets.

GameChatBalancer lets you use an Arduino (or similar boards) based knob or slider to **balance the volume of chat-software and games** so that you can **easily
tune the audio volume while playing** without any interruption by just turning a knob or moving a single slider.

The project consists of two components. A small **systray application** for your PC and an Arduino based **cheap to build** hardware.

# Features
1. Hardware
   - Noise reduction if you are using cheap components - especially the potentiometer
   - Only sends data via the serial port if something changed - no permanent datastream

2. Systray Application
   - Easy configuration via GUI - no extra configuration file editing needed
   - Auto-detect the Arduino based hardware
   - 





# Licences and Thanks
First of all I would like to thank **sverrirs for his Gist AudioManager.cs** which accelerated the development and for sure saved me from some pain during the development. I used and modified AudioManager.cs to my needs so that GameChatBalancer is able to adjust the volume of more than one application by application name in one single call.

Additionally I used some icons from https://icon-icons.com:
Designer | Iconpack | Icon
---------|----------|---------
Perpixel | Mobile Basic Vol. 1 | conversation Icon


# Next steps
- [x] Implement options for noise reduction (Off(0) - Low(2) - High(3))
  - [x] Default: High
  - [ ] Customizable via C# application
  
  https://stackoverflow.com/questions/43175454/how-to-get-selected-value-from-dropdown-list-c-sharp-asp-net

## Optional
- [ ] Screen overlay to see volume adjustments Ingame
