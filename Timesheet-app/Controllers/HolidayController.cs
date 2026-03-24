using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Services;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HolidayController : Controller
    {
        private readonly IHollidayService _hollidayService;
        public HolidayController(IHollidayService hollidayService)
        {
            _hollidayService = hollidayService;
        }
        [HttpPost("AddHoliday")]
        public async Task<ActionResult> AddHoliday([FromBody] HollidayDTO holiday)
        {
            if (holiday == null)
            {
                return BadRequest("Holiday data is required");
            }
            try
            {
                await _hollidayService.AddHoliday(holiday);
                return Ok("Holiday added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("GenerateRecuring")]
        public async Task<ActionResult> GenerateRecuring()
        {
            var currentYear = DateTime.Now.Year;
            try
            {
                await _hollidayService.GenerateRecuringHoliday(currentYear);
                return Ok($"Holidays for the year {currentYear} generated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
