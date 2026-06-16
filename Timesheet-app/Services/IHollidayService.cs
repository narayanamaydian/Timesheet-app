using Timesheet_app.Models;
using Timesheet_app.Models.DTOs;

namespace Timesheet_app.Services
{
    public interface IHollidayService
    {
        public Task<HollidayDTO?> GetHolidayByDate(DateOnly date);
        public Task AddOrUpdate(HollidayDTO holiday);
        public Task<List<HollidayDTO>> GenerateRecuringHoliday(int year);

    }
}
