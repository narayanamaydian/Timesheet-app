using System.Data;
using Timesheet_app.Models;
using Timesheet_app.Models.DAO;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface ITimesheetService
    {
        Task<TimesheetUserDTO> GetTimesheetByMonthAsync(string userId, int? month, int year);
        Task<TimesheetUserDTO> GetTimesheetByUserAsync(string userId, int? month);
        Task<TimesheetDTO> AddTimesheetAsync(TimesheetDTO timesheet, string userId);
        Task<List<TimesheetDTO>> GenerateTimesheetForMonthAsync(string userId, int month, int year);
        Task<DataTable> GetTimesheetByMonthForUsersAsync(int month, int year, string userId);
        Task<TimesheetDTO> AddOrUpdateClockIn(string userId, int status);
        Task<TimesheetDTO> AddOrUpdateClockOut(string userId, string activity);
    }
}