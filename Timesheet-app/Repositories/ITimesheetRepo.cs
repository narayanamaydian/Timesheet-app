using Timesheet_app.Models;
using Timesheet_app.Models.DAO;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Repositories
{
    public interface ITimesheetRepo
    {
        public Task<List<TimesheetModel>> GetTimesheetByMonth(string userId, int? month, int year);

        public Task<TimesheetModel> GetTimesheetByDay(string userId, int? day, int month, int year);
        public Task<TimesheetModel> GetTimesheetById(string id);

        public Task AddTimesheet(TimesheetModel timesheet);
        public Task AddOrUpdateClockIn(TimeOnly clockIn, string timesheetId, WorkStatus workStatus);
        public Task AddOrUpdateClockOut(TimeOnly clockOut, TimeSpan accumulatedTime, string timesheetId);

        public Task UpdateTimesheet(TimesheetModel timesheet);

        
    }
}
