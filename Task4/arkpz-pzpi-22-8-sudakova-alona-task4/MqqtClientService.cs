using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
public class MqttClientService
{
    private readonly IMqttClient _mqttClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MqttClientService> _logger;

    public MqttClientService(
        IServiceScopeFactory scopeFactory,
        ILogger<MqttClientService> logger)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();
        _scopeFactory = scopeFactory;
        _logger = logger;

        // Підписуємось на отримання повідомлень
        _mqttClient.ApplicationMessageReceivedAsync += HandleMessageAsync;
    }

    // Підключення до брокера
    public async Task ConnectAsync()
    {
        if (!_mqttClient.IsConnected)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("34.243.217.54", 1883) // Оновлено адресу брокера
                .Build();

            await _mqttClient.ConnectAsync(options);

            // Підписуємось на топік після підключення
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("focuslearn/sessions")
                .Build();

            await _mqttClient.SubscribeAsync(topicFilter);
            _logger.LogInformation("Connected to MQTT broker and subscribed to topics");
        }
    }

    // Обробка отриманих повідомлень
    private async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        try
        {
            var payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            _logger.LogInformation($"Received message: {payload}");

            var sessionData = JsonSerializer.Deserialize<IoTSessionDTO>(payload);

            using (var scope = _scopeFactory.CreateScope())
            {
                var sessionService = scope.ServiceProvider.GetRequiredService<IIoTSessionService>();
                var result = await sessionService.SaveIoTSessionAsync(sessionData);

                if (!result)
                {
                    _logger.LogError("Failed to save session data");
                }
                else
                {
                    _logger.LogInformation($"Successfully saved session for user {sessionData.UserId}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MQTT message");
        }
    }

    // Публікація повідомлення
    public async Task PublishMessageAsync(string topic, string payload)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                await ConnectAsync();
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .Build();

            await _mqttClient.PublishAsync(message);
            _logger.LogInformation($"Message sent: {topic}, Payload: {payload}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message: {Message}", ex.Message);
        }
    }

    // Метод для відключення
    public async Task DisconnectAsync()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
            _logger.LogInformation("Disconnected from MQTT broker");
        }
    }
}