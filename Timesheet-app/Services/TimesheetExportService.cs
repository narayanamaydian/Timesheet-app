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

            var listColumns = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            string name = dt.Rows[0][listColumns[0]] != DBNull.Value ? dt.Rows[0][listColumns[0]].ToString() : "Unknown";
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

            var headerColumns = ScanFileAndReturnHeaderAsync(worksheet, out int headerRow);

            var headerColumnsString = string.Join(", ", headerColumns);

            // 3. Template kamu mulai data di row 9


            var countNotWorkingDays = 0;

            int rowIndex = headerRow + 1;

            foreach (DataRow row in dt.Rows)
            {
                var totalDaysInMonth = daysInMonth + 7;
                if (rowIndex > totalDaysInMonth)
                {
                    worksheet.Row(rowIndex - 1).CopyTo(worksheet.Row(rowIndex));
                }
                //foreach (var column in listColumns)
                //{
                //this is my data
                //    { "User ID", "Name", "Vendor Name", "No SPK", "Date", "Clock In", "Clock Out", "Accumulated Time", "WFO", "Working", "Holiday Description", "Activity", "ProjectName" }
                // this is my excel header
                //    Nama, Vendor, SPK, Tanggal, Flexy Hour Start, Flexy Hour End, Hour, Project ID - Project Name, Activity, WFO/WFH
                //    AddDataExcel(worksheet, rowIndex, row, column);
                //}

                AddDataExcel(worksheet, rowIndex, row, listColumns[0]);
                AddDataExcel(worksheet, rowIndex, row, listColumns[1]);
                AddDataExcel(worksheet, rowIndex, row, listColumns[2]);
                AddDataExcel(worksheet, rowIndex, row, listColumns[3]);
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
                    AddDataExcel(worksheet, rowIndex, row, listColumns[4]);
                    AddDataExcel(worksheet, rowIndex, row, listColumns[5]);
                    AddDataExcel(worksheet, rowIndex, row, listColumns[6]);
                    AddDataExcel(worksheet, rowIndex, row, listColumns[7]);
                    AddDataExcel(worksheet, rowIndex, row, listColumns[8]);
                    AddDataExcel(worksheet, rowIndex, row, listColumns[9]);
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

        private static void AddDataExcel(IXLWorksheet worksheet, int rowIndex, DataRow row, string columnName)
        {
            worksheet.Cell(rowIndex, 1).Value = XLCellValue.FromObject(row[columnName]);
            worksheet.Cell(rowIndex, 1).Style.Font.Bold = false;
            worksheet.Cell(rowIndex, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private static List<string> ScanFileAndReturnHeaderAsync(IXLWorksheet worksheet, out int headerRow)
        {
            var headers = new List<string>();
            int foundHeaderRow = 1;
            headerRow = 0;

            int columnNumber = 1;
            bool headerFound = false;

            // Scan columns until we find a non-empty row
            while (!headerFound && columnNumber <= 100)
            {
                // Scan rows 1-100 in the current column
                for (int row = 1; row <= 100; row++)
                {
                    var cell = worksheet.Cell(row, columnNumber);

                    if (!cell.Value.IsBlank && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        // Found a non-empty cell, this row is the header row
                        headerFound = true;
                        foundHeaderRow = row;

                        // Extract all headers from this row across all columns
                        int colIndex = 1;
                        while (true)
                        {
                            var headerCell = worksheet.Cell(row, colIndex);
                            if (headerCell.Value.IsBlank || string.IsNullOrWhiteSpace(headerCell.Value.ToString()))
                            {
                                break;
                            }
                            headers.Add(headerCell.Value.ToString());
                            colIndex++;
                        }
                        break;
                    }
                }

                // Move to next column if header not found
                if (!headerFound)
                {
                    columnNumber++;
                }
            }

            headerRow = foundHeaderRow;
            return headers;
        }
    }
}
