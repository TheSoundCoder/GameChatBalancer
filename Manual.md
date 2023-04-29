# GameChatBalancer - Installation

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

# Manual - Systray application
The application will start in the systray:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/Manual/GCB_Systray_Expanded.png)

A right click will open the context menu. The first row shows the **connection status** to your DIY hardware device. The second row shows the **last recieved value** from the hardware.

Clicking on **Show** will open the GUI so that you can configure GameChatBalancer:

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/Manual/GCB_settings.png)

The two symbols on the left ("Equalizer" and "Bug") let you switch between the [Settings](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#settings) and [Debug](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#debug) area.

## Settings
The settings area is your **command center** for this application. It has two main areas:
- Hardware properties
- Audio settings

We will start with the hardware properties as this might be of interest on the first startup.

### Hardware properties
![](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/assets/Manual/GCB_settings_hwproperties.png)

#### ComPort
In the Hardware properties area, you can choose the **COM port** for your DIY hardware. Usually it should be a COM port greater than COM2. **I recomment to leave this setting to "Auto"**. In this case GameChatBalancer will auto-detect your DIY hardware by doing a basic handshake (syn <> ack).

If your device is properly connected the checkbox "Connected" will be ticked. Otherwise in a disconnected state this checkbox will be empty. 

#### Noise reduction
Cheap potentiometers / sliders can cause noise.

**What is noise?** - The Arduino / Seeed XIAO microcontroller reads the analog resistor value of oyur potentiometer resulting in values between 0-1023. If you do not touch your potentiometer this value should be static. If you turn your potentiometer to the middle position this value should be ~512. A cheap potentiometer has a higher tolerance meaning that this value will not be static but will vary for eyample between 508 and 516.

**Why is noise bad?** - To reduce the traffic on the Com port and to reduce the CPU load of your PC, the Arduino sketch is implemented in a way so that a new value is only sent to the pc if it does not exactly match the value before. No turn on the potentiometer -> No change in values -> No data is sent to the PC -> GameChantBalancer Systray application keeps sleeping.
Noise will cause a constant stream of new values between the Arduino and your PC and might cause an increased CPU load as the volume settings for applications are constantly modified.

### Audio settings
You can fully control and customize GameChatBalancer to your needs in the settings area.
If your DIY hardware is connected to the PC GameChatBalancer should automatically detect it and connect to it. 


## Debug
