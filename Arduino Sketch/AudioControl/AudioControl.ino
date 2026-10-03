int analogPin = A0; // potentiometer wiper (middle terminal) connected to analog pin 0

int val1 = 0;       // current potentiometer value (0-100)
int val1_old = 0;  // previous value
String incomingMsg = "";

// Noise Reduction:
// 0 = disabled (1 sample)
// 1 = Low    (16 samples)
// 2 = Medium (32 samples)
// 3 = High   (64 samples)
//
// DO NOT CHANGE.
// This can be modified using the Systray application.
int intNRLevel = 3;
int samples = 64;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  analogReadResolution(10);
}

void loop() {
  // Read potentiometer using the configured amount of samples
  val1 = readPotentiometer();
  // Convert ADC value (0-1023) to balancing value (0-100)
  val1 = map(val1, 0, 1023, -2, 202);
  // Only send value if it has changed
  if (abs(val1 - val1_old) >=2) {
    val1_old = val1;
    if (val1>200){val1=200;}  //only send values <= 100
    if (val1<0){val1=0;} //do not send negative values
    Serial.println(val1/2);
  }
  // Check for incoming serial commands
  Incoming();
}

/*
 * Reads the potentiometer multiple times and returns
 * the average ADC value.
 */
int readPotentiometer() {
  long sum = 0;
  for (int i = 0; i < samples; i++) {
    sum += analogRead(analogPin);
  }
  return sum / samples;
}

int setSample(){
/*
 * Noise Reduction levels:
 * 0 = 1 sample
 * 1 = 16 samples
 * 2 = 32 samples
 * 3 = 64 samples
 */
  switch (intNRLevel) {
    case 1:
      samples = 16;
      break;
    case 2:
      samples = 32;
      break;
    case 3:
      samples = 64;
      break;
    case 0:
    default:
      samples = 1;
      break;
  }
}

/*
 * Handle incoming serial commands.
 */
void Incoming() {
  if (Serial.available() > 0) {
    incomingMsg = Serial.readString();
    // Used to autodetect the device
    if (incomingMsg.indexOf("syn") > -1) {
      Serial.println("ack");
    }
    // Send current value on request
    if (incomingMsg.indexOf("get") > -1) {
      Serial.println(val1);
    }
    // Set and acknowledge Noise Reduction level
    if (incomingMsg.indexOf("NR=") > -1) {
      int newNRLevel = incomingMsg.substring(3, 4).toInt();
      // Only accept valid Noise Reduction levels
      if (newNRLevel >= 0 && newNRLevel <= 3) {
        intNRLevel = newNRLevel;
        setSample();
      }
      Serial.println("NR=" + String(intNRLevel));
    }
  }
}
