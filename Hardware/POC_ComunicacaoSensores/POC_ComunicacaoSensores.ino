#include <MPU6050_tockn.h>
#include <Wire.h>

const int flexPin1 = 36; 
const int flexPin2 = 39; 
const int flexPin3 = 34;
const int flexPin4 = 35;
const int flexPin5 = 32;
const int piezoPin1 = 33;
const int piezoPin2 = 25;
int sensorReadX, sensorReadY, sensorReadZ = 0;

int16_t ax, ay, az;
int16_t accX, accY, accZ;
 
 MPU6050 mpu6050(Wire);

void setup() 
{ 
  Serial.begin(115200);

  Wire.begin(21, 22, 9600);
  mpu6050.begin();
  //mpu6050.calcGyroOffsets(true); // Use essa linha para calibrar o sensor antes de iniciar o programa.
} 
 
void loop() 
{ 
  int flexValue1, flexValue2, flexValue3, flexValue4, flexValue5;
  int piezoValue1, piezoValue2;

  ax = mpu6050.getAccAngleX();
  ay = mpu6050.getAccAngleY();
  //az = mpu6050.get();
  //accX = mpu6050.getAccX();
  //accY = mpu6050.getAccY();
  //accZ = mpu6050.getAccZ();

  mpu6050.update();
  // Se pegarmos todas as características do sensor acelerômetro/giroscópio, será necessário 14 bytes (1 byte por característica)
  // sensorReadX = mpu6050.getAngleX();  //read the value of the potentiometer
  // sensorReadY = mpu6050.getAngleY();  //read the value of the potentiometer
  // sensorReadZ = mpu6050.getAngleZ();  //read the value of the potentiometer

  //sensorReadX1 = mpu6050.readMPU6050(byte reg)

  flexValue1 = analogRead(flexPin1);
  flexValue2 = analogRead(flexPin2);  
  flexValue3 = analogRead(flexPin3);
  flexValue4 = analogRead(flexPin4);
  flexValue5 = analogRead(flexPin5);
  piezoValue1 = analogRead(piezoPin1);
  piezoValue2 = analogRead(piezoPin2);

  Serial.print("x:\t");
  Serial.print(ax); Serial.print("\t");
  Serial.print("y:\t");
  Serial.print(ay); Serial.println("\t");
  //Serial.print("z:\t");
  //Serial.print(az); Serial.println("\t");
  Serial.print("x:\t");
  Serial.print(accX); Serial.print("\t");
  Serial.print("y:\t");
  Serial.print(accY); Serial.print("\t");
  Serial.print("z:\t");
  Serial.print(accZ); Serial.println("\t");
  Serial.print("sensor 1: "); Serial.println(flexValue1); Serial.print("\t");
  Serial.print("sensor 2: "); Serial.println(flexValue2); Serial.print("\t");
  Serial.print("sensor 3: "); Serial.println(flexValue3); Serial.print("\t");
  Serial.print("sensor 4: "); Serial.println(flexValue4); Serial.print("\t");
  Serial.print("sensor 5: "); Serial.println(flexValue5); Serial.print("\t");
  Serial.print("sensor Piezoeletrico 1: "); Serial.println(piezoValue1); Serial.print("\t");
  Serial.print("sensor Piezoeletrico 2: "); Serial.println(piezoValue2); Serial.print("\t");
  
  delay(500);
} 