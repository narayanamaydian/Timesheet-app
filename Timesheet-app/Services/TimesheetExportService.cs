using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using System.Data;
using Timesheet_app.Helper;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public class TimesheetExportService : ITimesheetExportService
    {
        private readonly ITimesheetService _timesheetService;
        private readonly IConfiguration _configuration;

        public TimesheetExportService(ITimesheetService timesheetService, IConfiguration configuration)
        {
            _timesheetService = timesheetService;
            _configuration = configuration;
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
            worksheet.Cell(rowIndex, 10).Value = "Activity";

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
            worksheet.Column(10).Width = 16; // Activity

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
                    worksheet.Cell(rowIndex, 10).Value = XLCellValue.FromObject(row["Activity"]);
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

        public async Task<ExcelFileDto> GenerateTimesheetFromTemplateAsync(string userId, int month, int year)
        {
            // 1. Ambil data timesheet
            var generatedTimesheet = await _timesheetService.GenerateTimesheetForMonthAsync(userId, month, year);
            var dt = await _timesheetService.GetTimesheetByMonthForUsersAsync(month, year, userId);
            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException("No timesheet data found for the specified user and period.");

            // 2. Ambil path template
            var templatePath = _configuration["FileLocation:Templates"];
            var templateFileName = "templateTimesheet.xlsx";
            var fullTemplatePath = Path.Combine(templatePath, templateFileName);

            string name = dt.Rows[0]["Name"] != DBNull.Value ? dt.Rows[0]["Name"].ToString() : "Unknown";
            var date = new DateTime(year, month, 1);

            var periodDate = date.ToString("MMM-yy");

            // Total days in the month
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Template starts at row 9
            int templateStartRow = 9;

            // Template has 30 rows (9–38)
            int templateEndRow = templateStartRow + 30 - 1;

            // Last row that should contain data
            int lastDataRow = templateStartRow + daysInMonth - 1;


            if (!File.Exists(fullTemplatePath))
                throw new FileNotFoundException($"Template file not found at path: {fullTemplatePath}");

            using var workbook = new XLWorkbook(fullTemplatePath);
            var worksheet = workbook.Worksheet(1);

            // 3. Template kamu mulai data di row 9


            var countNotWorkingDays = 0;

            int rowIndex = 9;

            foreach (DataRow row in dt.Rows)
            {
                var totalDaysInMonth = daysInMonth + 7;
                if(rowIndex > totalDaysInMonth)
                {
                    worksheet.Row(rowIndex -1).CopyTo(worksheet.Row(rowIndex));
                }

                worksheet.Cell(rowIndex, 1).Value = XLCellValue.FromObject(row["Name"]);
                worksheet.Cell(rowIndex, 2).Value = XLCellValue.FromObject(row["Vendor Name"]);
                worksheet.Cell(rowIndex, 3).Value = XLCellValue.FromObject(row["No SPK"]);
                worksheet.Cell(rowIndex, 4).Value = XLCellValue.FromObject(row["Date"]);
                if (row.Table.Columns.Contains("Working") &&
                   row["Working"] != DBNull.Value &&
                   !Convert.ToBoolean(row["Working"]))
                {
                    if (row.Table.Columns.Contains("Holiday Description") &&
                        row["Holiday Description"] != DBNull.Value &&
                        !string.IsNullOrEmpty(row["Holiday Description"].ToString()))
                    {

                        worksheet.Range(rowIndex, 5, rowIndex, 10).Merge();
                        worksheet.Cell(rowIndex, 5).Value = row["Holiday Description"].ToString();
                        worksheet.Range(rowIndex, 4, rowIndex, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1A983");
                    }
                    else
                    {
                        worksheet.Range(rowIndex, 5, rowIndex, 10).Value = "";
                        worksheet.Range(rowIndex, 4, rowIndex, 10).Style.Fill.BackgroundColor = XLColor.FromHtml("#808080");
                    }
                    

                    countNotWorkingDays++;
                }
                else
                {
                    worksheet.Cell(rowIndex, 5).Value = XLCellValue.FromObject(row["Clock In"]);
                    worksheet.Cell(rowIndex, 6).Value = XLCellValue.FromObject(row["Clock Out"]);
                    worksheet.Cell(rowIndex, 7).Value = XLCellValue.FromObject(row["Accumulated Time"]);
                    worksheet.Cell(rowIndex, 8).Value = XLCellValue.FromObject(row["ProjectName"]);
                    worksheet.Cell(rowIndex, 9).Value = XLCellValue.FromObject(row["Activity"]);
                    worksheet.Cell(rowIndex, 10).Value = XLCellValue.FromObject(row["WFO"]);
                }
                worksheet.Range(rowIndex, 1, rowIndex, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                rowIndex++;
            }

            

            // Delete extra rows
            if (lastDataRow < templateEndRow)
            {
                int rowsToDelete = templateEndRow - lastDataRow;
                worksheet.Rows(lastDataRow + 1, templateEndRow).Delete();
            }


            var countWorkingDays = rowIndex -9 - countNotWorkingDays;

            worksheet.Cell("C6").Value = name;
            worksheet.Cell("I1").Value = periodDate;
            worksheet.Cell("I5").Value = countWorkingDays;
            worksheet.Cell("I2").Value = countWorkingDays;


            // 4. Save ke MemoryStream
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

        /// <summary>
        /// Reads the Excel template file from the configured template location.
        /// Reads headers from range A8:J8 and content starting from row 9.
        /// Maps Excel data directly to TimesheetExcelDTO objects.
        /// Template file path: FileLocation:Templates/templateTimesheet.xlsx
        /// </summary>
        public async Task<List<TimesheetExcelDTO>> ReadTemplateFile()
        {
            var templatePath = _configuration["FileLocation:Templates"];
            if (string.IsNullOrWhiteSpace(templatePath))
                throw new InvalidOperationException("Template file path is not configured in appsettings.json (FileLocation:Templates).");

            var templateFileName = "templateTimesheet.xlsx";
            var fullTemplatePath = Path.Combine(templatePath, templateFileName);

            if (!File.Exists(fullTemplatePath))
                throw new FileNotFoundException($"Template file not found at path: {fullTemplatePath}");

            var excelDataList = new List<TimesheetExcelDTO>();

            using (var workbook = new XLWorkbook(fullTemplatePath))
            {
                var worksheet = workbook.Worksheet(1);

                // Read content rows starting from row 9 (headers are in row 8)
                int contentRow = 9;
                while (!worksheet.Cell(contentRow, 1).IsEmpty())
                {
                    worksheet.Cell(contentRow, 1).Value = worksheet.Cell(contentRow, 1).GetString().Trim();
                    var excelDto = new TimesheetExcelDTO
                    {
                        Nama = GetCellValue(worksheet, contentRow, 1),
                        Vendor = GetCellValue(worksheet, contentRow, 2),
                        SPK = GetCellValue(worksheet, contentRow, 3),
                        Tanggal = GetCellValue(worksheet, contentRow, 4),
                        FlexyHourStart = GetCellValue(worksheet, contentRow, 5),
                        FlexyHourEnd = GetCellValue(worksheet, contentRow, 6),
                        Hour = GetCellValue(worksheet, contentRow, 7),
                        ProjectIdProjectName = GetCellValue(worksheet, contentRow, 8),
                        Activity = GetCellValue(worksheet, contentRow, 9),
                        WfoWfh = GetCellValue(worksheet, contentRow, 10)
                    };

                    excelDataList.Add(excelDto);
                    contentRow++;
                }
            }

            return await Task.FromResult(excelDataList);
        }

        /// <summary>
        /// Gets the value from an Excel cell and converts it to string.
        /// Returns empty string if cell is empty.
        /// </summary>
        private string GetCellValue(IXLWorksheet worksheet, int row, int column)
        {
            var cell = worksheet.Cell(row, column);
            return cell.IsEmpty() ? string.Empty : cell.Value.ToString();
        }

        


    }
}
