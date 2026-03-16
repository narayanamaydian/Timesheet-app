using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Models;
using Timesheet_app.Models.DTOs;
using Timesheet_app.Repositories;

namespace Timesheet_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetRepo _timesheetRepo;
        private readonly IUserRepo _userRepo;
        public TimesheetController(ITimesheetRepo timesheetRepo, IUserRepo userRepo)
        {
            _timesheetRepo = timesheetRepo;
            _userRepo = userRepo;
        }
        [HttpGet("GetTimesheetByMonth/{userId}/{month}/{year}")]
        public async Task<IActionResult> GetTimesheetByMonth(string userId, int month, int year)
        {
            var timesheets = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            return Ok(timesheets);
        }

        [HttpGet("GetTimesheetByUser/{userId}")]
        public async Task<IActionResult> GetTimesheetByUser(string userId, [FromQuery] int? month)
        {
            month ??= DateTime.Now.Month;
            var year = DateTime.Now.Year;

            var timesheets = await _timesheetRepo.GetTimesheetByMonth(userId, month, year);
            return Ok(timesheets);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddTimesheet([FromBody] TimesheetModel timesheet)
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
                await _timesheetRepo.AddTimesheet(timesheet);
                return CreatedAtAction(nameof(AddTimesheet), new { id = timesheet.Id }, timesheet);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Generate")]
        public async Task<IActionResult> GenerateTimesheet([FromBody] TimesheetDto request)
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
                var timesheets = GeneratedForMonth(request.Id, request.Month, request.Year);
                foreach (var timesheet in timesheets)
                {
                    await _timesheetRepo.AddTimesheet(timesheet);
                }
                var currentMonth = DateTime.Now.Month;
                return Ok("Success insert in " + currentMonth);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private List<TimesheetModel> GeneratedForMonth(string userId, int month, int year)
        {
            var timesheets = new List<TimesheetModel>();
            UserModel user = _userRepo.GetUserById(userId).Result;

            var daysInMonth = DateTime.DaysInMonth(year, month);

            var clockIn = new TimeOnly(9, 0, 0);
            var clockOut = new TimeOnly(17, 0, 0);
            var totalTime = clockOut.ToTimeSpan() - clockIn.ToTimeSpan();



            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateOnly(year, month, day);
                var isItWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
                timesheets.Add(new TimesheetModel
                {
                    User = user,
                    Date = new DateOnly(year, month, day),
                    ClockIn = clockIn,
                    ClockOut = clockOut,
                    AccumulatedTime = totalTime,
                    Working = !isItWeekend,
                    Id = $"{userId}_{currentDate:yyyyMMdd}"
                });
            }
            return timesheets;
        }
    }

    
}
