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
    public class LearningMaterialsController : ControllerBase
    {
        private readonly ILearningMaterialService _service;

        public LearningMaterialsController(ILearningMaterialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Отримати всі матеріали навчальні матеріали
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMaterials()
        {
            var materials = await _service.GetAllMaterialsAsync();
            return Ok(materials);
        }

        /// <summary>
        /// Отримати всі матеріали поточного користувача
        /// </summary>
        [HttpGet("my-materials")]
        public async Task<IActionResult> GetMyMaterials()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var userId = int.Parse(userIdClaim.Value);

            var allMaterials = await _service.GetAllMaterialsAsync();

            // Фільтруємо матеріали за ідентифікатором користувача
            var userMaterials = allMaterials
                .Where(a => a.CreatorId == userId)
                .ToList();

            if (userMaterials == null || !userMaterials.Any())
            {
                return NotFound("You don't have any materials.");
            }

            return Ok(userMaterials);
        }

        /// <summary>
        /// Отримати навчальний матеріал за ідентифікатором
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialById(int id)
        {
            var material = await _service.GetMaterialByIdAsync(id);

            if (material == null)
                return NotFound("Material not found.");

            return Ok(material);
        }

        /// <summary>
        /// Додати матеріал (доступно репетиторам та адміністраторам)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Tutor, Admin")]
        public async Task<IActionResult> AddMaterial([FromBody] LearningMaterialDTO materialDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");


            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var creatorId = int.Parse(userIdClaim.Value);

            
            await _service.AddMaterialAsync(creatorId, materialDto);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Редагувати матеріал (доступно репетиторам та адміністраторам)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Tutor, Admin")]
        public async Task<IActionResult> UpdateMaterial(int id, [FromBody] LearningMaterialDTO materialDto)
        {
            var result = await _service.UpdateMaterialAsync(id, materialDto);

            if (!result)
                return NotFound("Material not found.");

            return NoContent();
        }

        /// <summary>
        /// Видалити матеріал (доступно репетиторам та адміністраторам)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Tutor, Admin")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var result = await _service.DeleteMaterialAsync(id);

            if (!result)
                return NotFound("Material not found.");

            return NoContent();
        }
    }
}
