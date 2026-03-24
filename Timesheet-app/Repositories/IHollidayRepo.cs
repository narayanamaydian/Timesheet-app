using Timesheet_app.Models;

namespace Timesheet_app.Repositories
{
    public interface IHollidayRepo
    {
        public Task<List<HollidayModel>> GetAllYearHollidaysAsync(int year);
        public Task<List<HollidayModel>> GetHollidayByMonthAsync(int month);

        public Task<HollidayModel> GetHollidayByDateAsync(int day, int month, int year);
        public Task<List<HollidayModel>> GetRecuringHoliday();
        public Task AddHolliday(HollidayModel holiday);
    }
}
