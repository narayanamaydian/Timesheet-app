using MongoDB.Bson;
using MongoDB.Driver;
using Timesheet_app.Data;
using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public class TimesheetRepo : ITimesheetRepo
    {
        private readonly IMongoCollection<TimesheetModel> _timesheetCollection;
        public TimesheetRepo(DatabaseConnection dbConnection)
        {
            _timesheetCollection = dbConnection.GetDatabase().GetCollection<TimesheetModel>("Timesheets");
        }

        public Task<TimesheetModel> GetTimesheetByDay(string userId, int? day, int month, int year)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TimesheetModel>> GetTimesheetByMonth(string userId, int? month, int year)
        {
            var startDate = new DateOnly(year, month ?? DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            var filter = Builders<TimesheetModel>.Filter.And(
                Builders<TimesheetModel>.Filter.Eq(x => x.User_id, userId),
                Builders<TimesheetModel>.Filter.Gte(x => x.Date, startDate),
                Builders<TimesheetModel>.Filter.Lte(x => x.Date, endDate)
            );
            return await _timesheetCollection.Find(filter).ToListAsync();
        }
    }
    
    
}
