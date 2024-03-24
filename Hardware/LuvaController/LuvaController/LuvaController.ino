#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>
#include <MPU6050_tockn.h>
#include <Wire.h>

MPU6050 mpu6050(Wire);

/* Define the UUID for our Custom Service */
#define serviceID BLEUUID((uint16_t)0x1700)

const int flexPin1 = 36; 
const int flexPin2 = 39; 
const int flexPin3 = 34;
const int flexPin4 = 35;
const int flexPin5 = 32;
const int buttonPin1 = 33;
const int buttonPin2 = 25;
const int buttonPin3 = 26;
int flexValue1, flexValue2, flexValue3, flexValue4, flexValue5;
int buttonValue1, buttonValue2, buttonValue3;

BLECharacteristic sensores(
  BLEUUID((uint16_t)0x015),
  BLECharacteristic::PROPERTY_NOTIFY
);

/* This function handles the server callbacks */
bool deviceConnected = false;
class ServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* MyServer) {
      deviceConnected = true;
      MyServer->startAdvertising();
    };

    void onDisconnect(BLEServer* MyServer) {
      deviceConnected = false;
      MyServer->startAdvertising();
    }
};

void setup() {
  Serial.begin(115200);

  // Cria e nomeia o dispositivo BLE
  BLEDevice::init("LuvaController");

  // Cria o servidor BLE
  BLEServer *MyServer = BLEDevice::createServer();
  MyServer->setCallbacks(new ServerCallbacks());  // Set the function that handles Server Callbacks

  // Adiciona um serviço ao servidor
  // Referência: https://btprodspecificationrefs.blob.core.windows.net/assigned-numbers/Assigned%20Number%20Types/Assigned_Numbers.pdf
  BLEService *customService = MyServer->createService(BLEUUID((uint16_t)0x015)); // Sensor

  // Adiciona uma característica ao serviço
  customService->addCharacteristic(&sensores);
  sensores.addDescriptor(new BLE2902());  //Adicionar essa linha apenas se houver a propriedade Notify

  BLEDescriptor VariableDescriptor(BLEUUID((uint16_t)0x0541)); //Sensor de movimento
  VariableDescriptor.setValue("Sensores Luva");
  sensores.addDescriptor(&VariableDescriptor);

  // Start the service
  customService->start();

  // Start the Server/Advertising
  MyServer->getAdvertising()->start();
  Serial.println("Waiting for a Client to connect...");

  Wire.begin(21, 22, 9600);
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  mpu6050.begin();
  mpu6050.calcGyroOffsets(true); // Use essa linha para calibrar o sensor antes de iniciar o programa.
}

void atualizaValoresSensores() {
  mpu6050.update();
  flexValue1 = analogRead(flexPin1);
  flexValue2 = analogRead(flexPin2);  
  flexValue3 = analogRead(flexPin3);
  flexValue4 = analogRead(flexPin4);
  flexValue5 = analogRead(flexPin5);
  buttonValue1 = digitalRead(buttonPin1);
  buttonValue2 = digitalRead(buttonPin2);
  buttonValue3 = digitalRead(buttonPin3);
}

char* trataValor(float valor) {
  char* aux = (char*)malloc(5); //+1 para o terminador nulo
  
  snprintf(aux, 5, "%04d", static_cast<int>(valor));

  return aux;
}

void enviaDados(char* resultado){
  int index = 0;
  int indexPacote = 0;

  String dataToSend(resultado);

  while (index < strlen(resultado)) {
    int remainingLength = strlen(resultado) - index;
    int chunkSize = min(remainingLength, 19); // Tamanho máximo do pacote BLE é 20 - Caractere inicial de Index
    char* aux = (char*)malloc(2); //+1 para o terminador nulo

    itoa(indexPacote, aux, 10);
    String pacoteCompleto = String(aux) + dataToSend.substring(index, index + chunkSize);

    sensores.setValue(pacoteCompleto.c_str());
    sensores.notify();  // Notifica o cliente

    index += chunkSize;
    indexPacote++;
    free(aux);
    delay(10); // Pequena pausa entre os pacotes
  }
}

void liberaMemoria(char* sensorFlex1, char* sensorFlex2, char* sensorFlex3, char* sensorFlex4, 
                   char* sensorFlex5, char* sensorAccX, char* sensorAccY, char* sensorGyrX, char* sensorGyrY, char* sensorGyrZ,
                   char* sensorButton1, char* sensorButton2, char* sensorButton3, char* resultado) {
  free(sensorFlex1);
  free(sensorFlex2);
  free(sensorFlex3);
  free(sensorFlex4);
  free(sensorFlex5);
  free(sensorAccX);
  free(sensorAccY);
  free(sensorGyrX);
  free(sensorGyrY);
  free(sensorGyrZ);
  free(sensorButton1);
  free(sensorButton2);
  free(sensorButton3);
  free(resultado);
}

void loop() {  
  if (deviceConnected) {
    atualizaValoresSensores();

    char* sensorFlex1 = trataValor(flexValue1);
    char* sensorFlex2 = trataValor(flexValue2);
    char* sensorFlex3 = trataValor(flexValue3);
    char* sensorFlex4 = trataValor(flexValue4);
    char* sensorFlex5 = trataValor(flexValue5);
    char* sensorAccX = trataValor(mpu6050.getAccAngleX());
    char* sensorAccY = trataValor(mpu6050.getAccAngleY());
    char* sensorGyrX = trataValor(mpu6050.getGyroX());
    char* sensorGyrY = trataValor(mpu6050.getGyroY());
    char* sensorGyrZ = trataValor(mpu6050.getGyroZ());
    char* sensorButton1 = trataValor(buttonValue1);
    char* sensorButton2 = trataValor(buttonValue2);
    char* sensorButton3 = trataValor(buttonValue3);

    // Alocando memória para a string resultante
    char* resultado = (char*)malloc(strlen(sensorFlex1) + strlen(sensorFlex2) + strlen(sensorFlex3) + 
                                    strlen(sensorFlex4) + strlen(sensorFlex5) + strlen(sensorGyrX) + 
                                    strlen(sensorGyrY) + strlen(sensorGyrZ) + strlen(sensorButton1) + 
                                    strlen(sensorAccX) + strlen(sensorAccY) + strlen(sensorButton2) +
                                    strlen(sensorButton3) + 14);  // +13 para pontos e vírgulas e +1 para terminador nulo
    
    // Concatenando as strings
    strcpy(resultado, sensorFlex1);
    strcat(resultado, ";");
    strcat(resultado, sensorFlex2);
    strcat(resultado, ";");
    strcat(resultado, sensorFlex3);
    strcat(resultado, ";");
    strcat(resultado, sensorFlex4);
    strcat(resultado, ";");
    strcat(resultado, sensorFlex5);
    strcat(resultado, ";");
    strcat(resultado, sensorAccX);
    strcat(resultado, ";");
    strcat(resultado, sensorAccY);
    strcat(resultado, ";");
    strcat(resultado, sensorGyrX);
    strcat(resultado, ";");
    strcat(resultado, sensorGyrY);
    strcat(resultado, ";");
    strcat(resultado, sensorGyrZ);
    strcat(resultado, ";");
    strcat(resultado, sensorButton1);
    strcat(resultado, ";");
    strcat(resultado, sensorButton2);
    strcat(resultado, ";");
    strcat(resultado, sensorButton3);
    strcat(resultado, ";");

    enviaDados(resultado);

    liberaMemoria(sensorFlex1, sensorFlex2, sensorFlex3, 
                  sensorFlex4, sensorFlex5, sensorAccX, 
                  sensorAccY, sensorGyrX, sensorGyrY, 
                  sensorGyrZ, sensorButton1, sensorButton2, 
                  sensorButton3, resultado);

    delay(100);
  }

  delay(100);
}