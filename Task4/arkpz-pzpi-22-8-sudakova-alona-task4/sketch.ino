#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_SSD1306.h>
#include <ArduinoJson.h>
#include <time.h>

// WiFi налаштування
const char* ssid = "Wokwi-GUEST";
const char* mqttServer = "34.243.217.54";
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
  WiFi.begin(ssid);
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

String getFormattedTime() {
  struct tm timeinfo;
  if (!getLocalTime(&timeinfo)) {
    Serial.println("Failed to obtain time");
    return "";
  }
  char timeString[24];
  strftime(timeString, sizeof(timeString), "%Y-%m-%dT%H:%M:%S", &timeinfo);
  return String(timeString);
}

// Надсилання даних сесії
void sendSessionData(const String& type, unsigned long startTime, unsigned long endTime) {
  StaticJsonDocument<200> sessionDoc;
  sessionDoc["SessionType"] = type;
  sessionDoc["StartTime"] = getFormattedTime();
  sessionDoc["EndTime"] = getFormattedTime();
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

// Основна функція для роботи і брейку
void startSession(const String& type, int durationMinutes) {
  unsigned long startTime = millis();
  unsigned long totalTime = durationMinutes * 60 * 1000;

  while (millis() - startTime < totalTime) {
    // Перевіряємо кнопку
    if (digitalRead(buttonPin) == LOW) {
      Serial.println("Кнопка натиснута. Завершуємо сесію.");
      sendSessionData(type, startTime, millis());
      delay(500); // Антибрязкіт
      return; // Вихід з функції
    }

    int secondsLeft = (totalTime - (millis() - startTime)) / 1000;
    String message = type + ": " + String(secondsLeft) + " sec left";
    displayMessage(message, 100); // Зменшена затримка для кращої реакції на кнопку

    if (!client.connected() || WiFi.status() != WL_CONNECTED) {
      reconnect();
    }
    client.loop(); // Важливо для обробки MQTT повідомлень
  }

  tone(buzzerPin, type == "Concentration" ? 500 : 800, 300);
  sendSessionData(type, startTime, millis());
  
  // Якщо завершилась сесія концентрації, починаємо перерву
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
