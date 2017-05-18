
const int pinR = 3;
const int pinG = 5;
const int pinB = 6;

String inputString = "";         // a string to hold incoming data
boolean stringComplete = false;  // whether the string is complete

void setup() {
  // initialize the digital pin as an output.
  pinMode(pinR, OUTPUT);     
  pinMode(pinG, OUTPUT); 
  pinMode(pinB, OUTPUT); 
  clearPins();

  // reserve 200 bytes for the inputString
  inputString.reserve(200);

  // initialize COM-port
  Serial.begin(9600);     
  Serial.println("Initialized...");
}

void loop() {
  if (stringComplete) {
    Serial.println("Command is:");
    Serial.println(inputString);
    runCommand(inputString);
    // clear the string
    inputString = "";
    stringComplete = false;
  }
}

void clearPins(){
  digitalWrite(pinR, HIGH);
  digitalWrite(pinG, HIGH);
  digitalWrite(pinB, HIGH);
}

void fading(int ledPin, boolean fade = false)  { 
  if (fade){
    // fade out from max to min in increments of 5 points
    for(int fadeValue = 255 ; fadeValue >= 0; fadeValue -=5) { 
      // sets the value (range from 0 to 255)
      analogWrite(ledPin, fadeValue);         
      // wait for 30 milliseconds to see the dimming effect    
      delay(30);
    } 

    // fade in from min to max in increments of 5 points
    for(int fadeValue = 0 ; fadeValue <= 255; fadeValue +=5) { 
      // sets the value (range from 0 to 255)
      analogWrite(ledPin, fadeValue);
      // wait for 30 milliseconds to see the dimming effect    
      delay(30);
    } 
  }
  else{
    digitalWrite(ledPin, LOW);
  }
}

/*
  SerialEvent occurs whenever a new data comes in the
 hardware serial RX.  This routine is run between each
 time loop() runs, so using delay inside loop can delay
 response.  Multiple bytes of data may be available.
 */
void serialEvent() {
  while (Serial.available()) {
    // get the new byte
    char inChar = (char)Serial.read();
    // add it to the inputString
    inputString += inChar;
    // if the incoming character is a newline, set a flag
    if (inChar == '\n') {
      inputString.trim();
      stringComplete = true;
    }
  }
}

void runCommand(String command) {
  if (command == "red") {
    Serial.println("Red");
    fading(pinR);
    return;
  }
  if (command == "green") {
    Serial.println("Green");
    fading(pinG);
    return;
  }
  if (command == "blue") {
    Serial.println("Blue");
    fading(pinB);
    return;
  }
  if (command == "fade red") {
    Serial.println("Fading red");
    fading(pinR, true);
    return;
  }
  if (command == "fade green") {
    Serial.println("Fading green");
    fading(pinG, true);
    return;
  }
  if (command == "fade blue") {
    Serial.println("Fading blue");
    fading(pinB, true);
    return;
  }
  Serial.println("Unknown command. Cleaning.");
  clearPins();
}
