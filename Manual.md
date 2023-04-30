# GameChatBalancer - Build

## What you need
You can find links to products I used and a rogh cost indication in the [Readme.md](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/README.md#material).

- 1 Arduino or compatible microcontroller. I recommend a **Seeed Xiao SAMD21 Cortex M0+**
- 1 Linear potentiometer with a low tolarance for the best result. I used a **Alpha RV16AF20KB10KM** (tolerance: +-5%) 
- 1 Knob, which will be set on top of the potentiometer
- 20-30 cm isolated copper wire
- 1 USB cable matching the connection of your microcontroller of choice. The Seeed board has a USB-C connector.
- soldering iron to connect the potentiometer to your microcontroller
- some kind of housing for the microcontroller and the potentiometer

## How to connect
- The potentiometer is connected to an **analog pin (A\*)** of your Arduino (or compatible) board. It is powered from the board's 5V (or 3.3V for a Seeed board) output (see schematic)
  - GND = black
  - 5V / 3.3V = red
  - Signal = green
- The board is conected via an USB cable to the PC.

### Schematic
In the schematic below, you find two options:
1. Left is the setup with the Seeed Xiao SAMD21 Cortex M0+. **Ensure to connect the potentiometer to the 3.3V output** of the board as the 5V will probably kill the board's microcontroller.
2. Right is the setup with an Arduino.

![](https://github.com/TheSoundCoder/AudioControl/blob/master/assets/GameChatBalancer_schematic_2.png)

If you connect everything exactly as shown in the schematic, the potentiometer will be connected to the **A0 pin** of your microcontroller. If you used another A? pin, you need to change the sketch before uploading it to your board. See [below](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#changeanaloguepin)

# GameChatBalancer - Installation

## Arduino
**Prerequisite**: You should have the **Arduino IDE** installed on your PC. You can download it [here](https://www.arduino.cc/en/software).
- Download the file **GCBalancer.ino** from the latest release of this project (you should find it at the right side under "Releases").
- Open the downloaded sketch AudioControl.ino with the installed Arduino IDE
- Follow the instructions "How to upload a sketch with the Arduino IDE 2" from the [Arduino website] (https://docs.arduino.cc/software/ide-v2/tutorials/getting-started/ide-v2-uploading-a-sketch)

### Change analog pin
You can change the used analog pin in **line 1** of the sketch. Change the value to whereever the **middle pin** of your potentiometer is connected to.

![Sketch](https://user-images.githubusercontent.com/130736237/235370434-3a052883-85e2-4aa0-8d43-80c4657d75c4.png)


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

A right click will open the context menu. The first row shows the **connection status** (COM4) to your DIY hardware device. The second row shows the **last recieved value** (59) from the hardware.

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

#### ComPort (blue)
In the Hardware properties area, you can choose the **COM port** for your DIY hardware. Usually it should be a COM port greater than COM2. **I recomment to leave this setting to "Auto"**. In this case GameChatBalancer will auto-detect your DIY hardware by doing a basic handshake (syn <> ack).

If your device is properly connected the checkbox "Connected" will be ticked. Otherwise in a disconnected state this checkbox will be empty. 

#### Noise reduction (green)
Cheap potentiometers / sliders can cause noise.

*What is noise?* - The Arduino / Seeed XIAO microcontroller reads the analog resistor value of your potentiometer resulting in values between 0 and 1023. If you do not touch your potentiometer this value should be static. If you turn your potentiometer to the middle position this value should be ~512. A cheap potentiometer has a higher tolerance meaning that this value will not be static but will vary for example between 508 and 516.

*Why is noise bad?* - To reduce the traffic on the Com port and to reduce the CPU load of your PC, the Arduino sketch is implemented in a way so that a new value is only sent to the pc if it does not exactly match the value before. No turn on the potentiometer -> No change in values -> No data is sent to the PC -> GameChantBalancer Systray application keeps sleeping.
Noise will cause a constant stream of new values between the Arduino and your PC and might cause an increased CPU load as the volume settings for applications are constantly modified.

*Which level of noise reduction should I use?* - If you want to validate if the chosen noise reduction level matches the quality of your potentiometer, use the  [Debug](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/Manual.md#debug) area as follows.
1. Enable debugging
2. Start with debug set to off.
3. Wait until the microcontroller confirms the chosen debug level. (Checkbox "NR confirmed" (Settings - Hardware properties) is checked.)
4. Change to the Debug area
5. **Do not turn the potentiometer or move the slider**
6. If the debug window shows a lot of altering values coming in, change to a higher noise reduction level.
7. Return with #3, except you already reached the noise reduction level "High". ;-)

*What is the downside of noise reduction?* - If noise reduction set to medium or high, the application will "drop" 1 value at level medium und 2 values (at level high) if yout change the direction when turning the potentiometer. The reason is that those values are buffered and not sent by the microcontroller. At level high it will look as follows (example).
- Initial value 50
- You turn the potentiometer left until you reach 45
- You start turing the potentiometer right. The first value will be 48 intead if 46 as 45, 46 and 47 are in the local buffer.


### Audio settings
You can fully control and customize GameChatBalancer to your needs in the settings area.
If your DIY hardware is connected to the PC GameChatBalancer should automatically detect it and connect to it. 


## Debug
You can see debug messages in the debug area of the application. To enable debug messages just activate the highlighted checkbox.

![](https://github.com/TheSoundCoder/GameChatBalancer/blob/master/assets/Manual/GCB_debug1.png)

