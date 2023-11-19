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

#include <HID-Project.h>
#include <HashMap.h>

#define MAP_SIZE 10

using ActuatorMapType = HashMap<int, int>;

void sendActuatorEvent(long actuatorId, long actuatorValue, long value3);
void evaluateAndSendActuatorValue(int actuatorId, int currentValue);

ActuatorMapType actuatorMap = ActuatorMapType(MAP_SIZE);
volatile boolean newSignal = false;
int potTolerance = 6;

void setup() {
  actuatorMap[0](1, 0);
  actuatorMap[1](2, 0);
  actuatorMap[2](3, 0);
}

void loop() {
  int pot1CurrentValue = analogRead(A0);
  int pot2CurrentValue = analogRead(A1);
  int pot3CurrentValue = analogRead(A2);
  
  evaluateAndSendActuatorValue(1, pot1CurrentValue);
  evaluateAndSendActuatorValue(2, pot2CurrentValue);
  evaluateAndSendActuatorValue(3, pot3CurrentValue);
  delay(100);
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

void evaluateAndSendActuatorValue(int actuatorId, int currentValue) {
    int actuatorIndex = actuatorMap.getIndexOf(actuatorId);
    if (actuatorIndex == -1) return;
    int storedValue = actuatorMap.getValueOf(actuatorId);
    // if (abs(storedValue - currentValue) > potTolerance) {
    if (currentValue <= storedValue - potTolerance || currentValue >= storedValue + potTolerance) {
        // String str1 = String("Actuator: ") + actuatorId + String("  New value: ") + currentValue + String("   Old value: ") + storedValue;
        // Serial.println(str1);
        actuatorMap[actuatorIndex].setValue(currentValue);
        sendActuatorEvent(actuatorId, currentValue, 0);
    }
}
