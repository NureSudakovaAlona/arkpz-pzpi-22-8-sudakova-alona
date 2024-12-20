using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IoTSessionController : ControllerBase
    {
        private readonly IIoTSessionService _service;

        public IoTSessionController(IIoTSessionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Додавання нової IoT-сесії
        /// </summary>
        [HttpPost("add-session")]
        public async Task<IActionResult> AddIoTSession([FromBody] IoTSessionDTO sessionDto)
        {
            if (sessionDto == null)
                return BadRequest("Invalid session data.");

            var result = await _service.AddIoTSessionAsync(sessionDto);
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

            var sessions = await _service.GetIoTSessionsByUserIdAsync(userId);
            if (sessions == null || !sessions.Any())
                return NotFound("No IoT sessions found.");

            return Ok(sessions);
        }
    }
}
