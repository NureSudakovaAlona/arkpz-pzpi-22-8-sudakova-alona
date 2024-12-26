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
    public class BusinessLogicController : ControllerBase
    {
        private readonly IBusinessLogicService _service;

        public BusinessLogicController(IBusinessLogicService service)
        {
            _service = service;
        }

        /// <summary>
        /// Отримати загальну статистику користувача
        /// </summary>
        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserStatistics([FromQuery] DateTime periodStartDate, [FromQuery] string periodType)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token");
                }

                var userId = int.Parse(userIdClaim.Value);
                var statistics = await _service.CalculateUserStatisticsAsync(userId, periodStartDate, periodType);

                return Ok(statistics);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while calculating statistics.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Оновити статистику методики для користувача
        /// </summary>
        [HttpPost("update-method-statistics")]
        public async Task<IActionResult> UpdateMethodStatistics([FromQuery] int methodId, [FromQuery] DateTime periodStartDate, [FromQuery] string periodType)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token");
                }

                var userId = int.Parse(userIdClaim.Value);
                await _service.UpdateMethodStatisticsAsync(userId, methodId, periodStartDate, periodType);

                return Ok(new { Message = "Statistics updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating statistics.", Details = ex.Message });
            }
        }

        /// <summary>
        /// Визначити найефективнішу методику
        /// </summary>
        [HttpGet("most-effective-method")]
        public async Task<IActionResult> GetMostEffectiveMethod([FromQuery] DateTime periodStartDate, [FromQuery] string periodType)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ??
                      User.FindFirst("sub") ??
                      User.FindFirst("UserId");

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID not found in token");
                }

                var userId = int.Parse(userIdClaim.Value);
                var result = await _service.GetMostEffectiveMethodAsync(userId, periodStartDate, periodType);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while determining the most effective method.", Details = ex.Message });
            }
        }
    }
}

