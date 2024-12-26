using FocusLearn.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //Дії доступні тільки адміністратору
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService adminService)
        {
            _service = adminService;
        }

        /// <summary>
        /// Змінити статус користувача
        /// </summary>
        [HttpPost("change-user-status")]
        public async Task<IActionResult> ChangeUserStatus([FromQuery] int userId, [FromQuery] string status)
        {
            if (status != "Active" && status != "Inactive")
                return BadRequest("Invalid status. Allowed values are 'Active' and 'Inactive'.");

            var result = await _service.ChangeUserStatusAsync(userId, status);
            if (!result)
                return NotFound("User not found.");

            return Ok($"User status changed to {status}.");
        }

        /// <summary>
        /// Зробити бекап БД
        /// </summary>
        [HttpPost("backup-database")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                var backupPath = await _service.BackupDatabaseAsync();
                return Ok($"Backup created successfully: {backupPath}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Експорт даних у форматі JSON
        /// </summary>
        [HttpPost("export-data")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportData([FromBody] List<string> tableNames)
        {
            try
            {
                // Викликаємо метод сервісу для експорту даних
                var exportedFilePaths = await _service.ExportDataAsync(tableNames);

                // Формуємо відповідь із шляхами до файлів
                return Ok(new
                {
                    Message = "Data exported successfully.",
                    ExportedFiles = exportedFilePaths
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while exporting data.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Імпорт даних користувачів
        /// </summary>
        [HttpPost("import-data")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportData([FromQuery] string tableName, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                return BadRequest(new { Message = "You need to enter a table name." });

            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "File was not upload or is empty." });

            try
            {
                var result = await _service.ImportDataAsync(tableName, file);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }
    }
}

