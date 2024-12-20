using FocusLearn.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FocusLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserMethodStatisticsController : ControllerBase
    {
        private readonly IUserMethodStatisticsService _service;

        public UserMethodStatisticsController(IUserMethodStatisticsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Отримати всю статистику
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStatistics()
        {
            var statistics = await _service.GetAllStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Отримати методику за ідентифікатором
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatisticsById(int id)
        {
            var statistics = await _service.GetStatisticsByIdAsync(id);

            if (statistics == null)
                return NotFound("Статистики не знайдено.");

            return Ok(statistics);
        }
    }
}
