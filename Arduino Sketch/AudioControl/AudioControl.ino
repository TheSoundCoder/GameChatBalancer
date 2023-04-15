int analogPin = A10; // potentiometer wiper (middle terminal) connected to analog pin 3
                    // outside leads to ground and +5V
int val1 = 0;  // variable to store the value read
int val1_old = 0;
String incomingMsg = "";
#define LastValues_SIZE 3 //the higher the number, the greate the noise reduction
int LastValues[LastValues_SIZE] = {-1,-1,-1};
//String message = "";

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);

}

void loop() {
  // put your main code here, to run repeatedly:
  val1 = analogRead(analogPin);  // read the input pin
  val1 = map(val1, 0, 1023, -2, 202);
  if (abs(val1 - val1_old)>1) {
    //only send something if the old and new value deviate from eachother
    val1_old = val1;  //store current value as old value
    if (val1>200){val1=200;}  //only send values <= 100
    if (val1<0){val1=0;} //do not send negative values
    if (!NoiseReduction(val1)) {Serial.println(String(val1/2));} //only send new value if it was not part of the last LastValues_SIZE values
  }
  Incoming();
}

bool NoiseReduction(int value){
  //noise reduction is true if nothin should be sent
  bool RetVal = true;
    if (!ValueIsPartOfArray(value)){
      //If the value is not part of our Array, shift array and add the new value
      RetVal = false;
      for (int i=0; i < LastValues_SIZE; i++) {
        if (i == LastValues_SIZE-1) {
          LastValues[i] = value;
        } else {
          LastValues[i] = LastValues[i+1];
        }
      }
    }
  return RetVal;
}

bool ValueIsPartOfArray(int value){
  //Return true if the value is part of an array, else: false
  bool RetVal = false;
  for (int i=0; i < LastValues_SIZE; i++) {
    if (LastValues[i] == value){RetVal = true;}
  }
  return RetVal;
}

void Incoming(){
  if (Serial.available() > 0) {
    // read the incoming byte:
    incomingMsg = Serial.readString();    
    if (incomingMsg.indexOf("syn")>-1){
      Serial.println("ack");
    } else if (incomingMsg.indexOf("get")>-1){
      Serial.println(String(val1/2));
    } else {
    }

  }
}

