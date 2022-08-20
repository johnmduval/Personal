//  Nilheim Mechatronics Simplified Eye Mechanism Code Nilheim机电一体化简化眼机构代码
//  Make sure you have the Adafruit servo driver library installed >>>>> https://github.com/adafruit/Adafruit-PWM-Servo-Driver-Library
//  X-axis joystick pin: A1 X轴操纵杆
//  Y-axis joystick pin: A0 Y轴操纵杆
//  Trim potentiometer pin: A2 调整电位器
//  Button pin: 2 按钮


#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();

#define SERVOMIN  140 // this is the 'minimum' pulse length count (out of 4096)这是“最小”脉冲长度计数
#define SERVOMAX  520 // this is the 'maximum' pulse length count (out of 4096)这是“最大”脉冲长度计数

// our servo # counter 我们的伺服计数器
uint8_t servonum = 0;

int xval;
int yval;

int lexpulse;//pulse脉冲
int rexpulse;

int leypulse;
int reypulse;

int uplidpulse;
int lolidpulse;
int altuplidpulse;
int altlolidpulse;

int trimval;

const int analogInPin = A0;
int sensorValue = 0;
int outputValue = 0;
int switchval = 0;

void setup() {
  Serial.begin(9600);
  Serial.println("8 channel Servo test!");
  pinMode(analogInPin, INPUT);
  pinMode(2, INPUT);

  pwm.begin();

  pwm.setPWMFreq(60);  // Analog servos run at ~60 Hz updates模拟伺服运行在~ 60hz更新

  delay(10);
}

// you can use this function if you'd like to set the pulse length in seconds如果你想以秒为单位设置脉冲长度，你可以使用这个功能
// e.g. setServoPulse(0, 0.001) is a ~1 millisecond pulse width. its not precise!例如，setservpulse(0, 0.001)是一个~1毫秒的脉冲宽度。它不准确!
void setServoPulse(uint8_t n, double pulse) {
  double pulselength;

  pulselength = 1000000;   // 1,000,000 us per second每秒一百万美元
  pulselength /= 60;   // 60 Hz
  Serial.print(pulselength); Serial.println(" us per period");
  pulselength /= 4096;  // 12 bits of resolution12位分辨率
  Serial.print(pulselength); Serial.println(" us per bit");
  pulse *= 1000000;  // convert to us转换为我们
  pulse /= pulselength;
  Serial.println(pulse);

}

void loop() {

  xval = analogRead(A1);
  lexpulse = map(xval, 0, 1023, 220, 440);
  rexpulse = lexpulse;

  switchval = digitalRead(2);


  yval = analogRead(A0);
  leypulse = map(yval, 0, 1023, 250, 500);
  reypulse = map(yval, 0, 1023, 400, 280);
  //leypulse = map(yval, 0, 1023, 280, 400);
  //reypulse = map(yval, 0, 1023, 400, 280);

  trimval = analogRead(A2);
  trimval = map(trimval, 320, 580, -40, 40);
  uplidpulse = map(yval, 0, 1023, 400, 280);//400,280
  uplidpulse -= (trimval - 40);
  uplidpulse = constrain(uplidpulse, 280, 400);
  altuplidpulse = 680 - uplidpulse;

  lolidpulse = map(yval, 0, 1023, 410, 280);
  lolidpulse += (trimval / 2);
  lolidpulse = constrain(lolidpulse, 280, 400);
  altlolidpulse = 680 - lolidpulse;


  pwm.setPWM(0, 0, lexpulse);
  pwm.setPWM(1, 0, leypulse);


  if (switchval == HIGH) {  //
    pwm.setPWM(2, 0, 400);
    pwm.setPWM(3, 0, 240);
    pwm.setPWM(4, 0, 240);
    pwm.setPWM(5, 0, 400);
  }
  else if (switchval == LOW) {  //
    pwm.setPWM(2, 0, uplidpulse);
    pwm.setPWM(3, 0, lolidpulse);
    pwm.setPWM(4, 0, altuplidpulse);
    pwm.setPWM(5, 0, altlolidpulse);
  }


  char strBuf[50];
  sprintf(strBuf, "lex=%d, rex=%d, ley=%d, rey=%d, trim = %d", lexpulse, rexpulse, leypulse, reypulse, trimval);
  Serial.println(strBuf);
  
  //Serial.println(trimval);

  delay(5000);

}
