using System.Data;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface ITimesheetExportService
    {
        Task<ExcelFileDto> GenerateTimesheetExcelAsync(string userId, int month, int year);

        Task<List<TimesheetExcelDTO>> ReadTemplateFile();
        Task<ExcelFileDto> GenerateTimesheetFromTemplateAsync(string userId, int month, int year);
    }
}
