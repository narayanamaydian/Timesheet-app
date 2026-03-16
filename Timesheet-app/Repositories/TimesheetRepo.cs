using Microsoft.EntityFrameworkCore;
using Timesheet_app.Data;
using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class TimesheetRepo : ITimesheetRepo
    {
        private readonly IConfiguration _configuration;
        private readonly SQLServerDBConnection _dbContext;

        public TimesheetRepo(IConfiguration configuration, SQLServerDBConnection dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public Task AddTimesheet(TimesheetModel timesheet)
        {
            var timesheetModel = _dbContext.Timesheets.Add(timesheet);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<TimesheetModel> GetTimesheetByDay(string userId, int? day, int month, int year)
        {
            var TimesheetDay = new DateOnly(year, month, day ?? DateTime.Now.Day);
            return await _dbContext.Timesheets
                .Include(t => t.User)
                .Where(t => t.User.Id == userId && t.Date == TimesheetDay)
                .FirstOrDefaultAsync();
        }

        public async Task<List<TimesheetModel>> GetTimesheetByMonth(string userId, int? month, int year)
        {
            var startDate = new DateOnly(year, month ?? DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _dbContext.Timesheets
                .Include(t => t.User)
                .Where(t => t.User.Id == userId && t.Date >= startDate && t.Date <= endDate)
                .OrderBy(t => t.Date)
                .ToListAsync();

            //var startDate = new DateOnly(year, month ?? DateTime.Now.Month, 1);
            //var endDate = startDate.AddMonths(1).AddDays(-1);
            //var filter = Builders<TimesheetModel>.Filter.And(
            //    Builders<TimesheetModel>.Filter.Eq(x => x.User_id, userId),
            //    Builders<TimesheetModel>.Filter.Gte(x => x.Date, startDate),
            //    Builders<TimesheetModel>.Filter.Lte(x => x.Date, endDate)
            //);
            //return await _timesheetCollection.Find(filter).ToListAsync();
        }

       
    }
    
    
}
