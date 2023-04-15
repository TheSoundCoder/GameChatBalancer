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
