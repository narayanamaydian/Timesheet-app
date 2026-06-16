using Microsoft.AspNetCore.Mvc;
using System.Data;
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
        private readonly ITimesheetExportService _exportService;

        public TimesheetController(ITimesheetService timesheetService, ITimesheetExportService exportService)
        {
            _timesheetService = timesheetService;
            _exportService = exportService;
        }

        [HttpGet("month/{userId}/{month}/{year}")]
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

        [HttpGet("user/{userId}")]
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

        [HttpPost("{userId}")]
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

        [HttpGet("export/{userId}")]
        public async Task<IActionResult> GenerateExcelData(string userId, [FromQuery] int month, [FromQuery] int year)
        {
            if (month == 0 || year == 0)
            {
                return BadRequest("Request data is required");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var excel = await _exportService.GenerateTimesheetExcelAsync(userId, month, year);
                return File(excel.Content, excel.ContentType, excel.FileName);
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
        [HttpPost("clockIn/{userId}")]
        public async Task<ActionResult> InsertClockIn(string userId, [FromBody] int status)
        {
            try
            {
                var timesheet = await _timesheetService.AddOrUpdateClockIn(userId, status);
                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPut("clockOut/{userId}")]
        public async Task<ActionResult> InsertClockOut(string userId, [FromBody] string activity)
        {
            try
            {
                var timesheet = await _timesheetService.AddOrUpdateClockOut(userId, activity);
                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPut("{id}")]
        public IActionResult UpdateTimesheet(string id, TimesheetDTO dto)
        {
            // service call
            return Ok();
        }

        [HttpGet("GetExcelTemplate")]
        public async Task<IActionResult> GetExcelTemplate()
        {
            try
            {
                var excel = await _exportService.ReadTemplateFile();
                return Ok(excel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("exportTemplate/{userId}")]
        public async Task<IActionResult> GenerateExcelFromTemplate(string userId, [FromQuery] int month, [FromQuery] int year)
        {
            if (month == 0 || year == 0)
            {
                return BadRequest("Request data is required");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var excel = await _exportService.GenerateTimesheetFromTemplateAsync(userId, month, year);
                return File(excel.Content, excel.ContentType, excel.FileName);
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

