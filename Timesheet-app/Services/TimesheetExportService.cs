using ClosedXML.Excel;
using System.Data;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public class TimesheetExportService : ITimesheetExportService
    {
        private readonly ITimesheetService _timesheetService;

        public TimesheetExportService(ITimesheetService timesheetService)
        {
            _timesheetService = timesheetService;
        }

        public async Task<ExcelFileDto> GenerateTimesheetExcelAsync(string userId, int month, int year)
        {
            var dt = await _timesheetService.GetTimesheetByMonthForUsersAsync(month, year, userId);

            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException("No timesheet data found for the specified user and period.");

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

            // simple styling: header background, bold, freeze row, adjust widths
            var headerRange = worksheet.Range(rowIndex, 1, rowIndex, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            worksheet.SheetView.FreezeRows(1);

            worksheet.Column(1).Width = 18; // User ID
            worksheet.Column(2).Width = 24; // Name
            worksheet.Column(3).Width = 20; // Vendor Name
            worksheet.Column(4).Width = 16; // No SPK
            worksheet.Column(5).Width = 14; // Date
            worksheet.Column(6).Width = 14; // Clock In
            worksheet.Column(7).Width = 14; // Clock Out
            worksheet.Column(8).Width = 18; // Accumulated Time
            worksheet.Column(9).Width = 10; // WFO

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
                    worksheet.Range(rowIndex, 6, rowIndex, 9).Merge();
                    if (row.Table.Columns.Contains("Holiday Description") &&
                        row["Holiday Description"] != DBNull.Value &&
                        !string.IsNullOrEmpty(row["Holiday Description"].ToString()))
                    {
                        worksheet.Cell(rowIndex, 6).Value = row["Holiday Description"].ToString();
                        worksheet.Cell(rowIndex, 6).Style.Fill.BackgroundColor = XLColor.PinkOrange;
                    }
                    else
                    {
                        worksheet.Cell(rowIndex, 6).Value = "";
                        worksheet.Cell(rowIndex, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#BFBFBF");
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

            var bytes = stream.ToArray();
            var fileName = $"Timesheet_{userId}_{month}_{year}.xlsx";

            return new ExcelFileDto
            {
                Content = bytes,
                FileName = fileName,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }
    }
}
