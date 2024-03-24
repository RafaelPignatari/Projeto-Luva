#include <MPU6050_tockn.h>
#include <Wire.h>

const int flexPin1 = 36; 
const int flexPin2 = 39; 
const int flexPin3 = 34;
const int flexPin4 = 35;
const int flexPin5 = 32;
const int buttonPin1 = 33;
const int buttonPin2 = 25;
const int buttonPin3 = 26;
int sensorReadX, sensorReadY, sensorReadZ = 0;

int16_t ax, ay, az;
int16_t accX, accY, accZ;
 
 MPU6050 mpu6050(Wire);

void setup() 
{ 
  Serial.begin(115200);

  Wire.begin(21, 22, 9600);
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(buttonPin3, INPUT);
  mpu6050.begin();
  mpu6050.calcGyroOffsets(true); // Use essa linha para calibrar o sensor antes de iniciar o programa.
} 
 
void loop() 
{ 
  int flexValue1, flexValue2, flexValue3, flexValue4, flexValue5;
  int buttonValue1, buttonValue2, buttonValue3;

  flexValue1 = analogRead(flexPin1);
  flexValue2 = analogRead(flexPin2);  
  flexValue3 = analogRead(flexPin3);
  flexValue4 = analogRead(flexPin4);
  flexValue5 = analogRead(flexPin5);
  buttonValue1 = digitalRead(buttonPin1);
  buttonValue2 = digitalRead(buttonPin2);
  buttonValue3 = digitalRead(buttonPin3);

  ax = mpu6050.getAccAngleX();
  ay = mpu6050.getAccAngleY();
  az = mpu6050.getGyroZ();

  mpu6050.update();

  Serial.print("x:\t");
  Serial.print(ax); Serial.print("\t");
  Serial.print("y:\t");
  Serial.print(ay); Serial.println("\t");
  Serial.print("z:\t");
  Serial.print(az); Serial.println("\t");
  // Serial.print("x:\t");
  // Serial.print(accX); Serial.print("\t");
  // Serial.print("y:\t");
  // Serial.print(accY); Serial.print("\t");
  // Serial.print("z:\t");
  // Serial.print(accZ); Serial.println("\t");
  Serial.print("sensor 1: "); Serial.println(flexValue1); Serial.print("\t");
  Serial.print("sensor 2: "); Serial.println(flexValue2); Serial.print("\t");
  Serial.print("sensor 3: "); Serial.println(flexValue3); Serial.print("\t");
  Serial.print("sensor 4: "); Serial.println(flexValue4); Serial.print("\t");
  Serial.print("sensor 5: "); Serial.println(flexValue5); Serial.print("\t");
  Serial.print("botão 1: "); Serial.println(buttonValue1); Serial.print("\t");
  Serial.print("botão 2: "); Serial.println(buttonValue2); Serial.print("\t");
  Serial.print("botão 3: "); Serial.println(buttonValue3); Serial.print("\t");
  
  delay(100);
} 