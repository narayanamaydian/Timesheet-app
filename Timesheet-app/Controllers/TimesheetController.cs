using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Data;
using System.Reflection;
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

        public TimesheetController(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
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
        [HttpGet("Excel/{userId}")]
        public async Task<ActionResult<DataTable>> GenerateExelData(string userId, [FromQuery] int month, [FromQuery] int year)
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
                var dt = await _timesheetService.GetTimesheetByMonthForUsersAsync(month, year, userId);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Timesheet");

                int rowIndex = 1;

                // headers
                worksheet.Cell(rowIndex, 1).Value = "User ID";
                worksheet.Cell(rowIndex, 2).Value = "Name";
                worksheet.Cell(rowIndex, 3).Value = "Vendor Name";
                worksheet.Cell(rowIndex, 4).Value = "No SPK";
                worksheet.Cell(rowIndex, 5).Value = "Date";
                worksheet.Cell(rowIndex, 6).Value = "Clock In";
                worksheet.Cell(rowIndex, 7).Value = "Clock Out";
                worksheet.Cell(rowIndex, 8).Value = "Accumulated Time";
                worksheet.Cell(rowIndex, 9).Value = "WFO";
                rowIndex++;

                // data
                foreach (DataRow row in dt.Rows)
                {
                    worksheet.Cell(rowIndex, 1).Value = XLCellValue.FromObject(row["User ID"]);
                    worksheet.Cell(rowIndex, 2).Value = XLCellValue.FromObject(row["Name"]);
                    worksheet.Cell(rowIndex, 3).Value = XLCellValue.FromObject(row["Vendor Name"]);
                    worksheet.Cell(rowIndex, 4).Value = XLCellValue.FromObject(row["No SPK"]);
                    worksheet.Cell(rowIndex, 5).Value = XLCellValue.FromObject(row["Date"]);

                    bool working = row.Table.Columns.Contains("Working") &&
                                   row["Working"] != DBNull.Value &&
                                   Convert.ToBoolean(row["Working"]);

                    if (!working)
                    {
                        // Merge the four cells visually
                        worksheet.Range(rowIndex, 6, rowIndex, 9).Merge();
                        if (row.Table.Columns.Contains("Holiday Description") &&
                            row["Holiday Description"] != DBNull.Value &&
                            !string.IsNullOrEmpty(row["Holiday Description"].ToString()))
                        {
                            worksheet.Cell(rowIndex, 6).Value = row["Holiday Description"].ToString();
                        }
                        else
                        {
                            worksheet.Cell(rowIndex, 6).Value = "";
                        }
                        worksheet.Cell(rowIndex, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    }
                    else
                    {
                        worksheet.Cell(rowIndex, 6).Value = XLCellValue.FromObject(row["Clock In"]);
                        worksheet.Cell(rowIndex, 7).Value = XLCellValue.FromObject(row["Clock Out"]);
                        worksheet.Cell(rowIndex, 8).Value = XLCellValue.FromObject(row["Accumulated Time"]);
                        worksheet.Cell(rowIndex, 9).Value = XLCellValue.FromObject(row["WFO"]);
                    }

                    rowIndex++;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;


                return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"Timesheet_{userId}_{month}_{year}.xlsx");

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
