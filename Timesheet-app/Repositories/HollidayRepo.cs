using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Timesheet_app.Data;
using Timesheet_app.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Timesheet_app.Repositories
{
    public class HollidayRepo : IHollidayRepo
    {
        private readonly SQLServerDBConnection _dbContext;
        public HollidayRepo(SQLServerDBConnection dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddHolliday(HollidayModel holiday)
        {
            _dbContext.Holidays.Add(holiday);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<HollidayModel>> GetAllYearHollidaysAsync(int year)
        {
            return await _dbContext.Holidays.Where(h => h.Year == year || h.Recurring).ToListAsync();
        }

        public async Task<HollidayModel?> GetHollidayByDateAsync(int day, int month, int year)
        {
            return await _dbContext.Holidays.FirstOrDefaultAsync(h => h.Day == day && h.Month == month && (h.Year == year || h.Recurring));
            
        }

        public Task<List<HollidayModel>> GetHollidayByMonthAsync(int month)
        {
            return _dbContext.Holidays.Where(h => h.Month == month).ToListAsync();
        }

        public Task<List<HollidayModel>> GetRecuringHoliday()
        {
            var currentYear = DateTime.Now.Year;
            return _dbContext.Holidays.Where(h => h.Year == currentYear - 1 && h.Recurring).ToListAsync();
        }
    }
}
