# GameChatBalancer - Manual

## Installation

### Arduino
**Prerequisite**: You should have the **Arduino IDE** installed on your PC. You can download it [here](https://www.arduino.cc/en/software).
- Download the file **GCBalancer.ino** from the latest release of this project (you should find it at the right side under "Releases").
- Open the downloaded sketch AudioControl.ino with the installed Arduino IDE
- Follow the instructions "How to upload a sketch with the Arduino IDE 2" from the [Arduino website] (https://docs.arduino.cc/software/ide-v2/tutorials/getting-started/ide-v2-uploading-a-sketch)

#### Test
After uploading the sketch, open the serial monitor of thr Arduino IDE by choosing *Tools* ->  *Serial Monitor* (or by pressing `strg` + `shift` + `M`).

If you turn the potentiometer (or move the slider) you should see **values between 0 and 100** coming in. If this is true: Congratulations! Your device is working as it should.

**Close the serial monitor.** Keeping it open might prevent you from uploading a sketch in the future.

### Systray application
- Download the file **GCBalancer_win.zip** from the latest release of this project (you should find it at the right side under "Releases").
- Store the content of the ZIP file in any folder of your choice.
- Run **GCBalancer.exe** from the folder above.
- **Optional**: Add GCBalancer.exe to **Autostart** if you want to run it at system startup.
  - Press `Win` + `R` to open the Run menu
  - Type shell:startup and press enter
  - Right click and create a new link to GCBalancer.exe

# Manual - Systray application
The application will start in the systray:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/Manual/GCB_Systray_Expanded.png)

A right click will open the context menu. Clicking on **Show** will open the GUI so that you can configure GameChatBalancer:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/Manual/GCB_settings.png)

The two symbols on the left ("Equalizer" and "Bug") let you switch between the [Settings](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#settings) and [Debug](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#debug) area.

## Settings

### Hardware properties

### Audio settings
You can fully control and customize GameChatBalancer to your needs in the settings area.
If your DIY hardware is connected to the PC GameChatBalancer should automatically detect it and connect to it. 


## Debug
