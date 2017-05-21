
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
    Serial.print("Command is: ");
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
    fading(pinR);
    Serial.println("Done: 'red'.");
    return;
  }
  if (command == "green") {
    fading(pinG);
    Serial.println("Done: 'green'.");
    return;
  }
  if (command == "blue") {
    fading(pinB);
    Serial.println("Done: 'blue'.");
    return;
  }
  if (command == "fade red") {
    fading(pinR, true);
    Serial.println("Done: 'fading red'.");
    return;
  }
  if (command == "fade green") {
    fading(pinG, true);
    Serial.println("Done: 'fading green'.");
    return;
  }
  if (command == "fade blue") {
    fading(pinB, true);
    Serial.println("Done: 'fading blue'.");
    return;
  }
  Serial.println("Unknown command. Cleaning.");
  clearPins();
  Serial.println("Done: 'cleaning'.");
}
