/*
   HID Event Structure
   1. "type"
        Type: string / char[]
        Values: "event", "message"

   2. "value1"
        Type: int
        Notes:
          Actuatorid in the case of an "event"

   3. "value2"
        Type: int
        Notes:
          Value in the case of an "event" from most actuators

   4. "value3"
        Type: int
*/
// =============================
// Libraries
// =============================
#include <HID-Project.h>
#include <HashMap.h>
#include <Adafruit_NeoPixel.h>

// =============================
// Pre-Processor Variables
// =============================
// Map Variables
#define MAP_SIZE 16

// LED Variables
#define LED_PIN 7
#define LED_COUNT 2
#define BRIGHTNESS 200

Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

void sendActuatorEvent(long actuatorId, long actuatorValue, long value3);
void evalActuator(String type, int currentValue);

HashMap<int, int> actuatorPinMap = HashMap<int, int>(MAP_SIZE);
HashMap<int, int> actuatorValueMap = HashMap<int, int>(MAP_SIZE);
volatile boolean newSignal = false;
int potTolerance = 6;

uint8_t incomingHIDBuffer[255];

int pots[5] = {100, 101, 102, 103, 104};
int momSwitches[5] = {200, 201, 202, 203, 204};

void setup() {
  RawHID.begin(incomingHIDBuffer, sizeof(incomingHIDBuffer));
  
  strip.begin();           // INITIALIZE NeoPixel strip object (REQUIRED)
  strip.show();            // Turn OFF all pixels ASAP
  strip.setBrightness(BRIGHTNESS);

  strip.setPixelColor(0, strip.Color(255, 255, 255)); 
  strip.setPixelColor(1, strip.Color(255, 255, 255));
  // strip.setPixelColor(2, strip.Color(255, 255, 255));

  strip.show();

  // For LED, // Pin 0 is main LED, the use addresses 1, 3, 5 for neopixel chaining
  // For LED, use neopixel array address

  // =======================
  // Actuator Groups
  // =======================
  actuatorPinMap[0](100, A0);
  actuatorPinMap[1](200, 3); // Digital Push Button pins unmapped
  actuatorPinMap[2](300, 0);

  actuatorPinMap[3](101, A1);
  actuatorPinMap[4](201, 4);
  actuatorPinMap[5](301, 1);

  actuatorPinMap[6](102, A2);
  actuatorPinMap[7](202, -1);
  actuatorPinMap[8](302, 2);

  actuatorPinMap[9](103, A3);
  actuatorPinMap[10](203, -1);
  actuatorPinMap[11](303, 3);

  actuatorPinMap[12](104, A9);
  actuatorPinMap[13](204, -1);
  actuatorPinMap[14](304, 4);

  actuatorValueMap[0](100, 0);
  actuatorValueMap[1](101, 0);
  actuatorValueMap[2](102, 0);
  actuatorValueMap[3](103, 0);
  actuatorValueMap[4](104, 0);

  actuatorValueMap[5](200, 1);
  actuatorValueMap[6](201, 1);
  actuatorValueMap[7](202, 1);
  actuatorValueMap[8](203, 1);
  actuatorValueMap[9](204, 1);
  
  pinMode(3, INPUT_PULLUP);
  pinMode(4, INPUT_PULLUP);
  Serial.begin(9600);
}

void loop() {
  for (int p = 0; p < 5; p++) {
    evalActuator("pot", pots[p]);
  }

  for (int m = 0; m < 5; m++) {
    evalActuator("mom_switch", momSwitches[m]);
  }

  parseHidEventV1();

  delay(50);
}

void parseHidEventV1 () {
  int bytesAvailable = RawHID.available();
  if (bytesAvailable > 0) {
      String message = ""; // Create a string to hold your message
      // Use a for loop to ensure you read all available bytes
      for (int i = 0; i < bytesAvailable; i++) {
          char c = (char)RawHID.read(); // Read a byte and cast it to a character
          message += c; // Append the character to your message string
      }
      Serial.println(message); // Print the complete message

      // Now, let's split the message and convert to integers
      String eventType = message.substring(0, message.indexOf(','));
      int firstCommaIndex = message.indexOf(',');
      int secondCommaIndex = message.indexOf(',', firstCommaIndex + 1);
      int thirdCommaIndex = message.indexOf(',', secondCommaIndex + 1);
      int fourthCommaIndex = message.indexOf(',', thirdCommaIndex + 1);
      
      // Extracting integer values from the CSV string
      int value1 = message.substring(firstCommaIndex + 1, secondCommaIndex).toInt();
      int value2 = message.substring(secondCommaIndex + 1, thirdCommaIndex).toInt();
      int value3 = message.substring(thirdCommaIndex + 1, fourthCommaIndex).toInt();
      int value4 = message.substring(fourthCommaIndex + 1).toInt();
      
      // Debugging: print the extracted values
      Serial.print("Event Type: ");
      Serial.println(eventType);
      Serial.print("Value 1: ");
      Serial.println(value1);
      Serial.print("Value 2: ");
      Serial.println(value2);
      Serial.print("Value 3: ");
      Serial.println(value3);
      Serial.print("Value 4: ");
      Serial.println(value4);

      if (eventType == "event") evalEvent(value1, value2, value3, value4);
  }
}

void evalEvent(int actuatorId, int value1, int value2, int value3) {
    // Get Actuator Pin
    int actPinIdx = actuatorPinMap.getIndexOf(actuatorId);
    if (actPinIdx == -1) return;
    int actPin = actuatorPinMap.getValueOf(actuatorId);
    if (actPin == -1) return;

    // LED
    strip.setPixelColor(actPin, strip.Color(value1, value2, value3));
    strip.show();
}

void parseHidEventV2() {
  if (RawHID.available() > 0) {
        String message = "";
        // Continuously read data until there's none left
        while (RawHID.available() > 0) {
            char c = (char)RawHID.read(); // Read a byte and cast it to a character
            message += c; // Append the character to your message string
        }
        if (message.length() > 0) {
            Serial.println(message); // Print the complete message
        }
    }
}

void sendActuatorEvent(long actuatorId, long actuatorValue, long value3) {
  byte data[64] = {};
  char type[10] = "event";

  memcpy(data, type, 10);
  memcpy(data + 10, &actuatorId, 4);
  memcpy(data + 14, &actuatorValue, 4);
  memcpy(data + 18, &value3, 4);

  RawHID.write(data, sizeof(data));
}

void evalActuator(String type, int actuatorId) {
    // Get Actuator Pin
    int actPinIdx = actuatorPinMap.getIndexOf(actuatorId);
    if (actPinIdx == -1) return;
    int actPin = actuatorPinMap.getValueOf(actuatorId);
    if (actPin == -1) return;

    // Get Actuator Stored Value
    int actuatorIndex = actuatorValueMap.getIndexOf(actuatorId);
    if (actuatorIndex == -1) return;
    int storedValue = actuatorValueMap.getValueOf(actuatorId);
   
    if (type == "pot") {
      int potVal = analogRead(actPin);
      if (potVal <= storedValue - potTolerance || potVal >= storedValue + potTolerance) {
          actuatorValueMap[actuatorIndex].setValue(potVal);
          sendActuatorEvent(actuatorId, potVal, 0);
      }
    }
    if (type == "mom_switch") {
      // If last value is 0 and current value is 0, early exit
      // If last value is 1 and current value is 1, early exit
      // If last value is 1 and current value is 0, set last value + send message
      // If last value is 0 and current value is 1, set last value
      int msVal = digitalRead(actPin);
      if (storedValue == msVal) return;
      actuatorValueMap[actuatorIndex].setValue(msVal);
      if (storedValue == 1 && msVal == 0) sendActuatorEvent(actuatorId, msVal, 0);
    }
}

// String str1 = String("Actuator: ") + actuatorId + String("  New value: ") + potVal + String("   Old value: ") + storedValue;
// Serial.println(str1);
