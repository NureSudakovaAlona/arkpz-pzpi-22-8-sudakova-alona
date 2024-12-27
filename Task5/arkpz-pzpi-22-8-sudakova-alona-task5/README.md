# Документація проєкту FocusLearn

## Огляд

FocusLearn – це програмна система, створена для покращення навчального процесу шляхом інтеграції IoT-пристроїв, управління навчальними сесіями та реалізації серверної бізнес-логіки. Проєкт включає функціонал для управління користувачами, методиками концентрації та аналітикою сесій. IoT-пристрої підключаються до системи через MQTT для моніторингу та взаємодії в реальному часі.

---

## Компоненти системи

### 1. **Серверна частина**
- **Технології**: ASP.NET Core з архітектурою REST API.
- **База даних**: Microsoft SQL Server, управління схемою та даними через Entity Framework Core.
- **Авторизація**: Використовується JWT-токени та підтримка OAuth 2.0 (Google, Facebook).
- **Бізнес-логіка**: Обробка статистики сесій, управління ролями користувачів та оцінка ефективності методик концентрації.
- **Інтеграція**: MQTT-протокол для обміну даними з IoT-пристроями.

### 2. **IoT-клієнт**
- **Платформа**: Мікроконтролер ESP32 з WiFi.
- **Бібліотеки**: PubSubClient, Adafruit SSD1306, Adafruit GFX, ArduinoJson.
- **Функціонал**:
  - Відображення типу сесії та часу, що залишився.
  - Сигналізація початку та завершення сесій через буззер або вібрацію.
  - Передача даних про сесії на сервер для збереження та аналізу.

---

## Вимоги до середовища

### Програмне забезпечення:
- **Операційна система**: Windows 10 або новіша.
- **Середовище розробки**: Visual Studio 2022 із підтримкою .NET 6.0.
- **Сервер бази даних**: Microsoft SQL Server.
- **IoT-розробка**: Arduino IDE із драйвером ESP32.

### Залежності:
- Сервер:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
  - `Microsoft.AspNetCore.Authentication.Google`
  - `Microsoft.AspNetCore.Authentication.Facebook`
- IoT:
  - `PubSubClient`
  - `Adafruit SSD1306`
  - `Adafruit GFX`
  - `ArduinoJson`

---

## Інструкція з розгортання

### Крок 1: Клонування репозиторію
Скористайтеся командою:
```bash
git clone https://github.com/NureSudakovaAlona/FocusLearn.git
```

### Крок 2: Налаштування серверної частини
1. Відкрийте проєкт у Visual Studio.
2. Налаштуйте `appsettings.json`:
    - **Підключення до бази даних**:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "YOUR_CONNECTION_STRING"
      }
      ```
    - **JWT-конфігурація**:
      ```json
      "Jwt": {
        "SecretKey": "YOUR_SECRET_KEY",
        "Issuer": "FocusLearnApp",
        "Audience": "FocusLearnUsers"
      }
      ```
    - **OAuth дані**:
      ```json
      "Authentication": {
        "Facebook": {
          "AppId": "YOUR_APP_ID",
          "AppSecret": "YOUR_APP_SECRET"
        },
        "Google": {
          "ClientId": "YOUR_CLIENT_ID",
          "ClientSecret": "YOUR_CLIENT_SECRET"
        }
      }
      ```
    - **MQTT конфігурація**:
      ```json
      "Mqtt": {
        "BrokerAddress": "broker.emqx.io",
        "BrokerPort": 1883,
        "ClientId": "FocusLearnServer"
      }
      ```

### Крок 3: Ініціалізація бази даних
1. Запустіть сервер за допомогою Visual Studio (`F5` або `dotnet run`).
2. Викличте метод:
   ```
   POST /api/Admin/restore-database
   ```
   За замовчуванням буде використаний файл із папки `Backups`.

### Крок 4: Запуск серверної частини
Переконайтеся, що сервер доступний за адресою:
```
https://localhost:7124/swagger/index.html
```

### Крок 5: Налаштування IoT-клієнта
1. Відкрийте код IoT у Arduino IDE.
2. Налаштуйте такі параметри:
    - **WiFi**:
      ```c++
      const char* ssid = "YOUR_WIFI_SSID";
      const char* password = "YOUR_WIFI_PASSWORD";
      ```
    - **MQTT**:
      ```c++
      const char* mqttServer = "broker.emqx.io";
      const int mqttPort = 1883;
      const char* mqttTopic = "focuslearn/concentration";
      ```
3. Завантажте код на ESP32.
4. Перевірте підключення до WiFi та MQTT.

### Крок 6: Тестування
1. Виконайте тестові запити через Swagger або Postman:
   ```
   /api/IoTSession/send-session
   ```
2. Підтвердьте взаємодію IoT-пристрою з сервером (зміна статусу на OLED, сигнали буззера, відправка сесій до бази даних).

---

## Ліцензія

Проєкт ліцензований під ліцензією MIT. Див. файл `LICENSE` для деталей.

---

## Контриб'ютори

- **Судакова Альона** - Розробка програмної системи та інтеграція компонентів.
