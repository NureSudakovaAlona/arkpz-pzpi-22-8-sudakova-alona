#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_SSD1306.h>
#include <ArduinoJson.h>
#include <time.h>

// WiFi налаштування
const char* ssid = "Wokwi-GUEST";
const char* password = "";
const char* mqttServer = "5.196.78.28";
const int mqttPort = 1883;
const char* mqttTopic = "focuslearn/concentration";
const char* sessionTopic = "focuslearn/sessions";

WiFiClient espClient;
PubSubClient client(espClient);

// OLED дисплей
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1);

// Піни
const int buzzerPin = 32;
const int buttonPin = 25;

// Дані сесії
int workDuration = 0;
int breakDuration = 0;
int methodId = 0;
int userId = 0;

// Ініціалізація WiFi
void setupWiFi() {
  WiFi.begin(ssid, password);
  Serial.print("Підключаємося до WiFi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println(" Успішно підключено до Wi-Fi!");
  Serial.print("ІР-адреса: ");
  Serial.println(WiFi.localIP());
}

// Обробка MQTT повідомлень
void mqttCallback(char* topic, byte* payload, unsigned int length) {
  String message = "";
  for (int i = 0; i < length; i++) {
    message += (char)payload[i];
  }

  StaticJsonDocument<200> doc;
  if (deserializeJson(doc, message) == DeserializationError::Ok) {
    workDuration = doc["WorkDuration"];
    breakDuration = doc["BreakDuration"];
    methodId = doc["MethodId"];
    userId = doc["UserId"];
    startSession("Concentration", workDuration);
  } else {
    Serial.println("Помилка десеріалізації JSON.");
  }
}

// Підключення до MQTT
void reconnect() {
  while (!client.connected()) {
    Serial.print("Підключаємося до MQTT...");
    String clientId = "ESP32Client-" + String(random(0xffff), HEX);
    if (client.connect(clientId.c_str())) {
      Serial.println(" Успішно підключено!");
      client.subscribe(mqttTopic);
    } else {
      Serial.print("Помилка: ");
      Serial.println(client.state());
      delay(5000);
    }
  }
}

// Функція відображення на дисплеї
void displayMessage(const String& message, int duration) {
  display.clearDisplay();
  display.setTextSize(1);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 10);
  display.println(message);
  display.display();
  delay(duration);
}

String getFormattedTime(time_t timestamp) {
  struct tm timeinfo;
  localtime_r(&timestamp, &timeinfo);
  char timeString[24];
  strftime(timeString, sizeof(timeString), "%Y-%m-%dT%H:%M:%S", &timeinfo);
  return String(timeString);
}

// Надсилання даних сесії
void sendSessionData(const String& type, time_t startTime, time_t endTime) {
  StaticJsonDocument<200> sessionDoc;
  sessionDoc["SessionType"] = type;
  sessionDoc["StartTime"] = getFormattedTime(startTime);
  sessionDoc["EndTime"] = getFormattedTime(endTime);
  sessionDoc["MethodId"] = methodId;
  sessionDoc["UserId"] = userId;

  String sessionData;
  serializeJson(sessionDoc, sessionData);
  
  if (client.publish(sessionTopic, sessionData.c_str())) {
    Serial.println("Дані сесії успішно надіслано: " + sessionData);
  } else {
    Serial.println("Помилка відправки даних сесії");
  }
}

// Основна функція для концентрації та перерви
void startSession(const String& type, int durationMinutes) {
  time_t sessionStartTime;
  time(&sessionStartTime); // Отримуємо початковий час в секундах
  
  unsigned long startMillis = millis();
  unsigned long totalTime = durationMinutes * 60 * 1000;

  while (millis() - startMillis < totalTime) {
    // Перевіряємо кнопку
    if (digitalRead(buttonPin) == LOW) {
      Serial.println("Кнопка натиснута. Завершуємо сесію.");
      time_t sessionEndTime;
      time(&sessionEndTime);
      sendSessionData(type, sessionStartTime, sessionEndTime);
      delay(500); // Антибрязкіт
      return; // Вихід з функції
    }

    int secondsLeft = (totalTime - (millis() - startMillis)) / 1000;
    String message = type + ": " + String(secondsLeft) + " sec left";
    displayMessage(message, 100);

    if (!client.connected() || WiFi.status() != WL_CONNECTED) {
      reconnect();
    }
    client.loop();
  }

  tone(buzzerPin, type == "Concentration" ? 500 : 800, 300);
  time_t sessionEndTime;
  time(&sessionEndTime);
  sendSessionData(type, sessionStartTime, sessionEndTime);
  
  if (type == "Concentration") {
    startSession("Break", breakDuration);
  }
}

// Ініціалізація
void setup() {
  Serial.begin(115200);
  pinMode(buzzerPin, OUTPUT);
  pinMode(buttonPin, INPUT_PULLUP);

  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println(F("SSD1306 allocation failed"));
    for (;;) ;
  }
  display.clearDisplay();
  display.display();

  setupWiFi();
  
  // Налаштування часу
  configTime(7200, 0, "pool.ntp.org"); // UTC + 2 години (для України)
  
  // Чекаємо на синхронізацію часу
  Serial.println("Очікуємо синхронізації часу...");
  struct tm timeinfo;
  while(!getLocalTime(&timeinfo)) {
    Serial.print(".");
    delay(1000);
  }
  Serial.println("\nЧас синхронізовано!");
  
  client.setServer(mqttServer, mqttPort);
  client.setCallback(mqttCallback);
  reconnect();
}

// Основний цикл
void loop() {
  if (WiFi.status() != WL_CONNECTED) setupWiFi();
  if (!client.connected()) reconnect();
  client.loop();
}