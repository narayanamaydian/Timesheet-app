using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Timesheet_app.Models;
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
                await _hollidayService.AddOrUpdate(holiday);
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

        [HttpPost("UploadHoliday")]
        public async Task<ActionResult> UploadHolidayExcel(IFormFile file)
        {
            if (file == null || file.Length < 1)
                return BadRequest("No File Uploaded");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            int count = 0;

            foreach(var row in worksheet.RowsUsed().Skip(1))
            {
                var holiday = new HollidayDTO()
                {
                    Day = row.Cell(1).GetValue<int>(),
                    Month = row.Cell(2).GetValue<int>(),
                    Year = row.Cell(3).GetValue<int>(),
                    Description = row.Cell(4).GetValue<string>(),
                    Recurring = row.Cell(5).GetValue<bool>()
                };
                await _hollidayService.AddOrUpdate(holiday);
                count++;
            }
            return Ok($"Success! Holiday added: {count} Items");
        }
    }
}
