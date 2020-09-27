
#include <TM1637Display.h>
// TM1637 Display connection PINs & initialization:
#define CLK 2
#define DIO 4
TM1637Display display = TM1637Display(CLK, DIO);

// L298n Dual H-bridge motor driver
int in1 = 9; //Declaring the pins where in1 in2 from the driver are wired 
int in2 = 8; //here they are wired with D9 and D8 from Arduino
int ConA = 10; //And we add the pin to control the speed after we remove its jumper 
               //Make sure it's connected to a pin that can deliver a PWM signal

// Motor speed control buttons
byte fasterButtonPin = 6;
int slowerButtonPin = 3;
int offButtonPin = 5;

unsigned long debounceDelay = 50;    // the debounce time; increase if get flickers

int motorSpeed = 0;

//int fasterButtonState = 0; 
//int fasterButtonStateLast = LOW;

int slowerButtonState = 0; 
int offButtonState = 0; 

class DebounceButton {
  private:
    int pin;
    int state;
    int lastState;
    long lastDebounceTime;  // the last time the output pin was toggled
    int motorSpeedDelta;
    String buttonName;
    
  public:
    DebounceButton(byte pin, int motorSpeedDelta, String buttonName){
      this->pin = pin;
      this->state = 0;
      this->lastState = LOW;
      this->lastDebounceTime = 0;
      this->motorSpeedDelta = motorSpeedDelta;
      this->buttonName = buttonName; 

      pinMode(this->pin, INPUT);
    }
    
    void check(){
      int reading = digitalRead(this->pin);
      if (reading != this->lastState){
          this->lastDebounceTime = millis();
      }
      if ((millis() - this->lastDebounceTime) > debounceDelay) {
        if (reading != this->state){
          this->state = reading;
          if (this->state == HIGH) {
            if (this->motorSpeedDelta == 0) {
              motorSpeed = 0;
            }
            else {
              motorSpeed = motorSpeed + this->motorSpeedDelta;
            }
            if (motorSpeed > 255){
              motorSpeed = 255;
            }
            if (motorSpeed < 0){
              motorSpeed = 0;
            }

            Serial.print(this->buttonName);
            Serial.print(": ");
            Serial.println(motorSpeed);
          }      
        }
      }
      this->lastState = reading;
    }
};

DebounceButton FasterBtn(6, 4, String("faster"));
DebounceButton SlowerBtn(3, -2, String("slower"));
DebounceButton StopBtn(5, 0, String("stop"));

void setup() {
  pinMode(in1, OUTPUT); //Declaring the pin modes, obviously they're outputs
  pinMode(in2, OUTPUT);
  pinMode(ConA, OUTPUT);

  Serial.begin(9600);
  Serial.println("Starting up dude");

  display.setBrightness(3);
  display.clear();
}

void SetMotorSpeed(){
  digitalWrite(in1, LOW);
  digitalWrite(in2, HIGH);
  analogWrite(ConA, motorSpeed);
}

void TurnMotorOff(){
  digitalWrite(in1, LOW);
  digitalWrite(in2, LOW);
  analogWrite(ConA,0);
}

void loop() {
  FasterBtn.check();
  SlowerBtn.check();
  StopBtn.check();

  if (motorSpeed == 0) {
    TurnMotorOff();
  }
  else {
    SetMotorSpeed();
  }

  display.showNumberDec(motorSpeed);
  //delay(2000);
}
