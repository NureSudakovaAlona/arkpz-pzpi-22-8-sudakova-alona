using FocusLearn.Models.DTO;
using FocusLearn.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FocusLearn.Repositories.Abstract;
using FocusLearn.Repositories.Implementation;
using System.Security.Claims;


namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Доступ лише для авторизованих користувачів
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Отримати користувача за ідентифікатором
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("User with inserted ID not found.");

            return Ok(user);
        }

        /// <summary>
        /// Отримати всіх користувачів (доступно лише адміністраторам)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Отримати всіх користувачів з роллю репетитора
        /// </summary>
        [HttpGet("tutors")]
        public async Task<IActionResult> GetAllTutors()
        {
            var users = await _service.GetAllTutorsAsync();
            return Ok(users);
        }

        /// <summary>
        /// Отримати інформацію з особистого профілю
        /// </summary>
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var user = await _service.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        /// <summary>
        /// Редагувати інформацію особистого профілю
        /// </summary>
        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDTO updateUserDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                                  User.FindFirst("sub") ??
                                  User.FindFirst("UserId");


            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var result = await _service.UpdateUserAsync(userId, updateUserDto);

            if (!result)
                return NotFound("User not found.");

            return NoContent();
        }

        /// <summary>
        /// Видалити особистий профіль
        /// </summary>
        [HttpDelete("my-profile")]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }
            var userId = int.Parse(userIdClaim.Value);


            var result = await _service.DeleteUserAsync(userId);

            if (!result)
                return NotFound("User not found.");

            return NoContent();
        }

    }
}

