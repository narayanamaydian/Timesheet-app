using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Models.DAO;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Services;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IUserService _userService;

        public TimesheetController(ITimesheetService timesheetService, IUserService userService)
        {
            _timesheetService = timesheetService;
            _userService = userService;
        }

        [HttpGet("GetTimesheetByMonth/{userId}/{month}/{year}")]
        public async Task<ActionResult<List<TimesheetDTO>>> GetTimesheetByMonth(string userId, int month, int year)
        {
            try
            {
                var timesheets = await _timesheetService.GetTimesheetByMonthAsync(userId, month, year);
                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetTimesheetByUser/{userId}")]
        public async Task<ActionResult<List<TimesheetDTO>>> GetTimesheetByUser(string userId, [FromQuery] int? month)
        {
            try
            {
                var timesheets = await _timesheetService.GetTimesheetByUserAsync(userId, month);
                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add/{userId}")]
        public async Task<ActionResult<TimesheetDTO>> AddTimesheet(string userId, [FromBody] TimesheetDTO timesheet)
        {
            if (timesheet == null)
            {
                return BadRequest("Timesheet data is required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _timesheetService.AddTimesheetAsync(timesheet, userId);
                return CreatedAtAction(nameof(AddTimesheet), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("generate")]
        public async Task<ActionResult<List<TimesheetDTO>>> GenerateTimesheet([FromBody] TimesheetInputDto request)
        {
            if (request == null)
            {
                return BadRequest("Request data is required");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var timesheets = await _timesheetService.GenerateTimesheetForMonthAsync(request.Id, request.Month, request.Year);
                return Ok(timesheets);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
