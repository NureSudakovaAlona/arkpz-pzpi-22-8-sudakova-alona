using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FocusLearn.Models.DTO;
using FocusLearn.Repositories.Abstract;
using System.Security.Claims;

namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Доступ лише для авторизованих користувачів
    [ServiceFilter(typeof(UserStatusFilter))]

    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _service;

        public AssignmentsController(IAssignmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Отримати всі завдання
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAssignments()
        {
            var assignments = await _service.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        /// <summary>
        /// Отримати всі завдання поточного користувача
        /// </summary>
        [HttpGet("my-assignments")]
        public async Task<IActionResult> GetMyAssignments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var allAssignments = await _service.GetAllAssignmentsAsync();

            // Фільтруємо завдання за ідентифікатором користувача
            var userAssignments = allAssignments
                .Where(a => a.StudentId == userId || a.TutorId == userId)
                .ToList();

            if (userAssignments == null || !userAssignments.Any())
            {
                return NotFound("You don't have any assignments.");
            }

            return Ok(userAssignments);
        }

        /// <summary>
        /// Отримати завдання за ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignmentById(int id)
        {
            var assignment = await _service.GetAssignmentByIdAsync(id);
            if (assignment == null)
                return NotFound($"Assignment with ID {id} not found.");

            return Ok(assignment);
        }

        /// <summary>
        /// Створити нове завдання
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Tutor, Admin")] // Лише репетитори або адміністратори можуть створювати завдання
        public async Task<IActionResult> AddAssignment([FromBody] AssignmentDTO assignmentDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");


            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var tutorId = int.Parse(userIdClaim.Value);


            await _service.AddAssignmentAsync(tutorId, assignmentDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Оновити завдання за ID
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Tutor, Admin")] // Лише репетитори або адміністратори можуть оновлювати завдання
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] AssignmentDTO assignmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var result = await _service.UpdateAssignmentAsync(id, assignmentDto);
            if (!result)
                return NotFound($"Assignment with ID {id} not found.");

            return NoContent();
        }

        /// <summary>
        /// Видалити завдання за ID
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Tutor, Admin")] // Лише адміністратор може видаляти завдання
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var result = await _service.DeleteAssignmentAsync(id);
            if (!result)
                return NotFound($"Assignment with ID {id} not found.");

            return NoContent();
        }
    }
}
