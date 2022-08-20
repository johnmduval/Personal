
#define trigPin_1 10
#define echoPin_1 11
#define trigPin_2 12
#define echoPin_2 13

// defines variables
int sensorNum = 1;
long duration; // variable for the duration of sound wave travel

int distances_Index_1 = 0;
int distances_1[5];
int distances_Index_2 = 0;
int distances_2[5];


void setup() {
  pinMode(trigPin_1, OUTPUT);
  pinMode(echoPin_1, INPUT);
  pinMode(trigPin_2, OUTPUT);
  pinMode(echoPin_2, INPUT);
  Serial.begin(9600);
}

void loop() {
  
  // Clears the trigPin condition
  int trigPin;
  int echoPin;
  int dist_index;
  int* dist_arr;
  if (sensorNum == 1) {
    trigPin = trigPin_1;
    echoPin = echoPin_1;
    dist_index = distances_Index_1;
    distances_Index_1++;
    dist_arr = distances_1;
    //sensorNum = 2;
  }
  else {
    trigPin = trigPin_2;
    echoPin = echoPin_2;    
    dist_index = distances_Index_2;
    distances_Index_2++;
    dist_arr = distances_2;
    sensorNum = 1;
  }
  
  
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  
  // Sets the trigPin HIGH (ACTIVE) for 10 microseconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);
  
  // Calculating the distance - Speed of sound wave divided by 2 (go and back)
  int distance = duration * 0.034 / 2;
  dist_arr[distances_Index_2 % 5] = distance;
  
  
  // Displays the distance on the Serial Monitor
  Serial.print("Distance #");
  Serial.print(sensorNum);
  Serial.print(": ");
  for (int i = 0; i < 5; i++) {
    Serial.print(distance);
    Serial.print(",");
  }
  Serial.println(" cm");

  delayMicroseconds(10000);
}
