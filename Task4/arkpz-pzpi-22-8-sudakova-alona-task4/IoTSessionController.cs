using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Security.Claims;

namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IoTSessionController : ControllerBase
    {
        private readonly IIoTSessionService _serviceIoT;
        private readonly IConcentrationMethodService _serviceMethod;
        private readonly MqttClientService _mqttClient;

        public IoTSessionController(IIoTSessionService serviceIoT, IConcentrationMethodService serviceMethod, MqttClientService mqttClient)
        {
            _serviceIoT = serviceIoT;
            _serviceMethod = serviceMethod;
            _mqttClient = mqttClient;
        }

        /// <summary>
        /// Додавання нової IoT-сесії
        /// </summary>
        [HttpPost("add-session")]
        public async Task<IActionResult> AddIoTSession([FromBody] IoTSessionDTO sessionDto)
        {
            if (sessionDto == null)
                return BadRequest("Invalid session data.");

            var result = await _serviceIoT.AddIoTSessionAsync(sessionDto);
            if (!result)
                return StatusCode(500, "Failed to add IoT session.");

            return Ok("IoT session added successfully.");
        }

        /// <summary>
        /// Отримання IoT-сесій користувача
        /// </summary>
        [HttpGet("user-sessions")]
        public async Task<IActionResult> GetUserSessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                                  User.FindFirst("sub") ??
                                  User.FindFirst("UserId");


            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var sessions = await _serviceIoT.GetIoTSessionsByUserIdAsync(userId);
            if (sessions == null || !sessions.Any())
                return NotFound("No IoT sessions found.");

            return Ok(sessions);
        }

        /// <summary>
        /// Збереження IoT-сесії
        /// </summary>
        [HttpPost("save-session")]
        public async Task<IActionResult> SaveIoTSession([FromBody] IoTSessionDTO sessionDto)
        {
            if (sessionDto == null)
                return BadRequest("Invalid session data.");

            var result = await _serviceIoT.SaveIoTSessionAsync(sessionDto);
            if (!result)
                return StatusCode(500, "Failed to save IoT session.");

            return Ok("IoT session saved successfully.");
        }

        /// <summary>
        /// Надсилання деталей обраної методики до ІоТ системи
        /// </summary>
        [HttpPost("send-session")]
        public async Task<IActionResult> SendSessionData(int methodId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                     User.FindFirst("sub") ??
                     User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var method = await _serviceMethod.GetMethodByIdAsync(methodId);

            if (method == null)
                return NotFound("Method not found.");

            // Формуємо JSON для передачі на IoT-пристрій
            var payLoad = new
            {
                WorkDuration = method.WorkDuration,
                BreakDuration = method.BreakDuration,
                UserId = userId,
                MethodId = methodId,
                SessionType = "Concentration"
            };

            await _mqttClient.PublishMessageAsync("focuslearn/concentration", System.Text.Json.JsonSerializer.Serialize(payLoad));

            return Ok("Session data sent to IoT device.");
        }

        // Структура даних, що передається
        public class SessionData
        {
            public int MethodId { get; set; }
            public int WorkDuration { get; set; }  // тривалість роботи в хвилинах
            public int BreakDuration { get; set; }  // тривалість перерви в хвилинах
        }
    }
}
