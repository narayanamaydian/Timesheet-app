using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface ITimesheetRepo
    {
        public Task<List<TimesheetModel>> GetTimesheetByMonth(string userId, int? month, int year);

        public Task<TimesheetModel> GetTimesheetByDay(string userId, int? day, int month, int year);
    }
}
