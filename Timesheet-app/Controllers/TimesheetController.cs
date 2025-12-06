using Microsoft.AspNetCore.Mvc;
using Timesheet_app.Repositories;

namespace Timesheet_app.Controllers
{
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetRepo _timesheetRepo;
        public TimesheetController(ITimesheetRepo timesheetRepo)
        {
            _timesheetRepo = timesheetRepo;
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



    }
}
